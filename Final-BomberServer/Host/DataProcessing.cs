﻿using Final_BomberServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer.Host
{
    partial class GameServer
    {
        void DataProcessing(byte data, ref Client client)
        {
            switch (data)
            {
                case (byte)RMT.NeedMap:
                    ReceiveNeedMap(client);
                    break;
                case (byte)RMT.Ready:
                    Console.WriteLine("[Client #" + client.ClientId + "]Ready !");
                    ReceiveReady(client, buffer.ReadString(), buffer.ReadString());
                    break;
                case (byte)RMT.MoveDown:
                    Console.WriteLine("[Client #" + client.ClientId + "]Move down !");
                    ReceiveMovePlayer(client, Player.ActionEnum.WalkingDown);
                    break;
                case (byte)RMT.MoveLeft:
                    Console.WriteLine("[Client #" + client.ClientId + "]Move left !");
                    ReceiveMovePlayer(client, Player.ActionEnum.WalkingLeft);
                    break;
                case (byte)RMT.MoveRight:
                    Console.WriteLine("[Client #" + client.ClientId + "]Move right !");
                    ReceiveMovePlayer(client, Player.ActionEnum.WalkingRight);
                    break;
                case (byte)RMT.MoveUp:
                    Console.WriteLine("[Client #" + client.ClientId + "]Move up !");
                    ReceiveMovePlayer(client, Player.ActionEnum.WalkingUp);
                    break;
                case (byte)RMT.Standing:
                    Console.WriteLine("[Client #" + client.ClientId + "]Standing !");
                    ReceiveMovePlayer(client, Player.ActionEnum.Standing);
                    break;
                case (byte)RMT.PlaceBomb:
                    ReceiveBombPlacing(client);
                    break;

            }
        }
    }
}