using System.Drawing;
using System.Diagnostics;

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
            // Target represents the coordinates the player wants to shoot at
            Point target = UI.MakeTargetPoint(pointsShot, playerName);
            // The point is added to a player's list of points they've shot at
            pointsShot.Add(target);
            Debug.WriteLine(playerName + " shoots at " + target);
            UI.ReturnInformation(target, ShootOpponent(target));
        }
        public override void SetShips()
        {
            Ship ship;
            bool correctlyPlaced;
            bool lastShipCorrectlyPlaced = false;
            // Places all ships from settings
            for (int i = 0; i < Settings.shipCount; i++)
            {
                correctlyPlaced = false;
                while (!correctlyPlaced)
                {
                    // Gets a ship from player
                    ship = UI.GetShips(lastShipCorrectlyPlaced, playerName);
                    // Validates ship location and places if possible
                    correctlyPlaced = board.PlaceShips(ship);
                    lastShipCorrectlyPlaced = correctlyPlaced;
                }
            }
        }
    }
}
