﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BattleshipWeb
{
    public class Human : Player
    {
        private IUserInterface UI;
        public Human(string name, IUserInterface UI) : base(name)
        {
            this.UI = UI;
        }
        public override void YourTurn()
        {
            //target represents the coordinates the player wants to shoot at
            Point target = UI.MakeTargetPoint(points, playerName);
            //The point is added to a player's list of points they've shot at
            points.Add(target);
            UI.ReturnInformation(target, ShootOpponent(target));
        }
        public override void SetShips()
        {
            Ship ship;
            bool correctlyPlaced = false;
            bool lastShipCorrectlyPlaced = false;
            //Places five ships
            for (int i = 0; i < 5; i++)
            {
                correctlyPlaced = false;
                while (!correctlyPlaced)
                {
                    //Gets a ship from player
                    ship = UI.GetShips(lastShipCorrectlyPlaced, playerName);
                    //Validates ship location and places if possible
                    correctlyPlaced = board.PlaceShips(ship);
                    lastShipCorrectlyPlaced = correctlyPlaced;
                }
            }
        }
    }
}