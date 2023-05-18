﻿using Rhisis.Game.Common;
using Rhisis.Game.Entities;
using Rhisis.Game.Protocol.Packets.World.Server.Snapshots.Quests;
using Rhisis.Game.Resources;
using Rhisis.Game.Resources.Properties;
using Rhisis.Game.Resources.Properties.Quests;
using Rhisis.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.Game;

public sealed class QuestDiary
{
    private readonly Player _player;
    private readonly List<Quest> _quests = new();

    /// <summary>
    /// Gets the active quests.
    /// </summary>
    public IEnumerable<Quest> ActiveQuests => _quests.Where(x => !x.IsFinished);

    /// <summary>
    /// Gets the checked quests.
    /// </summary>
    public IEnumerable<Quest> CheckedQuests => ActiveQuests.Where(x => x.IsChecked);

    /// <summary>
    /// Gets the completed quests.
    /// </summary>
    public IEnumerable<Quest> CompletedQuests => _quests.Where(x => x.IsFinished);

    public QuestDiary(Player owner)
    {
        _player = owner;
    }

    /// <summary>
    /// Accepts and adds a new quest to the diary.
    /// </summary>
    /// <param name="questProperties">Quest properties.</param>
    /// <exception cref="ArgumentNullException">Thrown when the given quest is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the quest already exists in the diary.</exception>
    public void AcceptQuest(QuestProperties questProperties)
    {
        if (questProperties is null)
        {
            throw new ArgumentNullException(nameof(questProperties), "Cannot add an undefined quest.");
        }

        if (_quests.Any(x => x.Id == questProperties.Id))
        {
            throw new InvalidOperationException($"Quest '{questProperties.Id}' for player with id '{_player.Id}' already exists.");
        }

        Quest quest = new(questProperties, _player)
        {
            StartTime = DateTime.UtcNow
        };

        _quests.Add(quest);
        SendSetQuestPacket(_player, quest);
        _player.SendDefinedText(DefineText.TID_EVE_STARTQUEST, $"\"{GameResources.Current.GetText(quest.Properties.Title)}\"");
    }

    /// <summary>
    /// Completes the given quest.
    /// </summary>
    /// <param name="quest"></param>
    public void CompleteQuest(Quest quest)
    {
        if (quest is null)
        {
            throw new ArgumentNullException(nameof(quest), $"Cannot finish an undefined quest.");
        }

        // Check if player has enough space for reward items.
        if (quest.Properties.Rewards.Items != null && quest.Properties.Rewards.Items.Any())
        {
            IEnumerable<QuestItemProperties> itemsForPlayer = quest.Properties.Rewards.Items.Where(x => x.Sex == _player.Appearence.Gender || x.Sex == GenderType.Any);

            if (_player.Inventory.GetStorageCount() + itemsForPlayer.Count() > _player.Inventory.Capacity)
            {
                _player.SendDefinedText(DefineText.TID_QUEST_NOINVENTORYSPACE);
                return;
            }

            foreach (QuestItemProperties rewardItem in itemsForPlayer)
            {
                ItemProperties rewardItemProperties = GameResources.Current.Items.Get(rewardItem.Id);

                if (rewardItemProperties is not null)
                {
                    Item item = new(rewardItemProperties)
                    {
                        Refine = rewardItem.Refine,
                        Element = rewardItem.Element,
                        ElementRefine = rewardItem.ElementRefine
                    };

                    _player.Inventory.CreateItem(item);
                    _player.SendDefinedText(DefineText.TID_GAME_REAPITEM, $"\"{item.Name}\"");
                }
            }
        }

        // Remove quest items from inventory
        if (quest.Properties.QuestEndCondition.Items != null && quest.Properties.QuestEndCondition.Items.Any())
        {
            foreach (QuestItemProperties questItem in quest.Properties.QuestEndCondition.Items)
            {
                if (questItem.Remove)
                {
                    if (questItem.Sex == GenderType.Any || questItem.Sex == _player.Appearence.Gender)
                    {
                        ItemContainerSlot inventoryItemSlot = _player.Inventory.FindSlot(x => x.HasItem && x.Item.Id == GameResources.Current.GetDefinedValue(questItem.Id));

                        if (inventoryItemSlot is not null)
                        {
                            _player.Inventory.DeleteItem(inventoryItemSlot, questItem.Quantity);
                        }
                    }
                }
            }
        }

        if (quest.Properties.Rewards.Gold > 0)
        {
            _player.Gold.Increase(quest.Properties.Rewards.Gold);
        }

        if (quest.Properties.Rewards.Experience > 0)
        {
            _player.Experience.Increase(quest.Properties.Rewards.Experience);
        }

        if (quest.Properties.Rewards.HasJobReward())
        {
            // TODO: set new job to player
        }

        if (quest.Properties.Rewards.Restat)
        {
            _player.ResetStatistics();
        }

        if (quest.Properties.Rewards.Reskill)
        {
            _player.ResetSkills();
        }

        if (quest.Properties.Rewards.SkillPoints > 0)
        {
            _player.AddSkillPoints(quest.Properties.Rewards.SkillPoints);
        }

        quest.IsFinished = true;
        quest.IsChecked = false;
        quest.State = QuestState.Completed;
        quest.EndTime = DateTime.UtcNow;

        _player.SendDefinedText(DefineText.TID_EVE_ENDQUEST, $"\"{GameResources.Current.GetText(quest.Properties.Title)}\"");
        SendSetQuestPacket(_player, quest);
    }

    /// <summary>
    /// Gets the quest by it's id.
    /// </summary>
    /// <param name="questId">Quest id to look for.</param>
    /// <returns>The quest if found; null otherwise.</returns>
    public Quest GetQuest(int questId) => _quests.FirstOrDefault(x => x.Id == questId);

    /// <summary>
    /// Gets the active quest by it's id.
    /// </summary>
    /// <param name="questId">Quest id to look for.</param>
    /// <returns>The quest if found; null otherwise.</returns>
    public Quest GetActiveQuest(int questId) => _quests.FirstOrDefault(x => x.Id == questId);

    /// <summary>
    /// Checks if the diary contains the quest identified by the given id.
    /// </summary>
    /// <param name="questId">Quest id.</param>
    /// <returns>True if the the diary contains the quest; false otherwise.</returns>
    public bool HasQuest(int questId) => _quests.Any(x => x.Id == questId);

    /// <summary>
    /// Checks if the diary contains the active quest identified by the given id.
    /// </summary>
    /// <param name="questId">Quest id.</param>
    /// <returns>True if the the diary contains the active quest; false otherwise.</returns>
    public bool HasActiveQuest(int questId) => ActiveQuests.Any(x => x.Id == questId);

    /// <summary>
    /// Removes the given quest from the diary.
    /// </summary>
    /// <param name="quest">Quest to remove.</param>
    public void Remove(Quest quest)
    {
        quest.IsDeleted = true;
    }

    /// <summary>
    /// Check if the player can start the given quest script.
    /// </summary>
    /// <param name="questProperties">Quest properties.</param>
    /// <returns>True if the player can start the quest; false otherwise.</returns>
    public bool CanStartQuest(QuestProperties questProperties)
    {
        if (questProperties is null)
        {
            return false;
        }

        if (HasQuest(questProperties.Id))
        {
            return false;
        }

        QuestProperties previousQuest = GameResources.Current.Quests.Get(questProperties.StartRequirements.PreviousQuestId);

        if (previousQuest is not null && !CompletedQuests.Any(x => x.Id == previousQuest.Id))
        {
            return false;
        }

        if (_player.Level < questProperties.StartRequirements.MinLevel || _player.Level > questProperties.StartRequirements.MaxLevel)
        {
            return false;
        }

        if (questProperties.StartRequirements.Jobs is not null && !questProperties.StartRequirements.Jobs.Contains(_player.Job.Id))
        {
            return false;
        }

        // TODO: add more checks

        return true;
    }

    /// <summary>
    /// Updates the diary with the given action;
    /// </summary>
    /// <param name="actionType">Quest action type.</param>
    /// <param name="values">Quest action values.</param>
    public void Update(QuestActionType actionType, params object[] values)
    {
        // TODO
    }

    /// <summary>
    /// Serializes the current diary into the given packet.
    /// </summary>
    /// <param name="packet">Packet.</param>
    public void Serialize(FFPacket packet)
    {
        packet.WriteByte((byte)ActiveQuests.Count());
        foreach (Quest quest in ActiveQuests)
        {
            quest.Serialize(packet);
        }

        packet.WriteByte((byte)CompletedQuests.Count());
        foreach (Quest quest in CompletedQuests)
        {
            packet.WriteInt16((short)quest.Id);
        }

        packet.WriteByte((byte)CheckedQuests.Count());
        foreach (Quest quest in CheckedQuests)
        {
            packet.WriteInt16((short)quest.Id);
        }
    }

    private static void SendSetQuestPacket(Player player, Quest quest)
    {
        using SetQuestSnapshot questSnapshot = new(player, quest);

        player.Send(questSnapshot);
    }
}
