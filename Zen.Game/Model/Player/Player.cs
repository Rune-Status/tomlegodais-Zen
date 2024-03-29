﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zen.Game.Model.Item;
using Zen.Game.Model.Player.Communication;
using Zen.Game.Msg;
using Zen.Game.Msg.Impl;
using Zen.Shared;

namespace Zen.Game.Model.Player
{
    public class Player : Mob.Mob
    {
        public Player(World world, string username, string password) : base(world, new Position(GameConstants.SpawnX, GameConstants.SpawnY))
        {
            Username = username;
            Password = password;
            Init();
        }

        public string Username { get; }
        public string Password { get; }
        public int Rights { get; set; } = 2;
        public DateTimeOffset CreationDateTime { get; set; } = DateTimeOffset.UtcNow;
        public PlayerSession Session { get; set; }
        public Appearance Appearance { get; } = Appearance.DefaultAppearance;
        public int[] AppearanceTickets { get; } = new int[2500];
        public List<Player> LocalPlayers { get; } = new List<Player>();
        public List<Npc.Npc> LocalNpcs { get; } = new List<Npc.Npc>();
        public bool RegionChanging { get; private set; }
        public Position LastKnownRegion { get; private set; }
        public InterfaceSet InterfaceSet { get; private set; }
        public ChatMessage ChatMessage { get; private set; }
        public SkillSet SkillSet { get; private set; }
        public ItemContainer Inventory { get; } = new ItemContainer(28);
        public ItemContainer Equipment { get; } = new ItemContainer(14);
        public PlayerSettings Settings { get; private set; }
        public ContactManager ContactManager { get; set; }

        public int Stance => Equipment.Get(EquipmentConstants.Weapon)?.EquipmentDefinition.Stance ?? 1426;

        private void Init()
        {
            /* Initialize members with instances of this player. */
            InterfaceSet = new InterfaceSet(this);
            SkillSet = new SkillSet(this);
            Settings = new PlayerSettings(this);
            ContactManager = new ContactManager(this);

            /* Register container listeners. */
            Inventory.AddListener(new ContainerMessageListener(this, 149, 0, 93));
            Inventory.AddListener(new ContainerFullListener(this, "inventory"));

            Equipment.AddListener(new ContainerMessageListener(this, 387, 28, 94));
            Equipment.AddListener(new ContainerFullListener(this, "equipment"));
            Equipment.AddListener(new ContainerAppearanceListener(this));
        }

        public new void Reset()
        {
            base.Reset();
            RegionChanging = false;
            ChatMessage = null;
        }

        public new void StopAction(bool cancelMoving)
        {
            base.StopAction(cancelMoving);
            if (InterfaceSet.InterfaceOpen)
            {
                InterfaceSet.RemoveOpenInterface();
            }
        }

        public void Logout()
        {
            var future = Send(new LogoutMessage());
            future?.ContinueWith(delegate { Session.Close(); });
        }

        public Task Send(IMessage message) => Session.Send(message);
        public bool IsChatUpdated() => ChatMessage != null;
        public void UpdateChatMessage(ChatMessage message) => ChatMessage = message;
        public void SendGameMessage(string text) => Send(new GameMessage(text));

        public void SetLastKnownRegion(Position lastKnownRegion)
        {
            LastKnownRegion = lastKnownRegion;
            World.GameMap.Parse(Position.RegionX, Position.RegionY);
            RegionChanging = true;
        }
    }
}