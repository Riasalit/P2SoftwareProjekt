using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using HAPI;
using System.Diagnostics;
using System.IO;
using BattleshipWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace BattleshipTest.ModelTest
{
    [TestClass]
    public class BoardTest
    {
        [TestMethod]
        public void CanBoardPlaceShipCorrectly()
        {
            Player testPlayer = new Human("TestHuman", new TestUI());

            testPlayer.board.PlaceShips(new Ship("TestShip", 3, new Point(2, 3), 'H'));

            Assert.AreEqual(true, testPlayer.board.ships != null);
            Assert.AreEqual(true, testPlayer.board.ships[0].shipCoord.X == 2);
            Assert.AreEqual(true, testPlayer.board.ships[0].shipCoord.Y == 3);
        }
        [TestMethod]
        public void CanBoardAvoidPlacingShipIfError()
        {
            Player testPlayer = new Human("TestHuman", new TestUI());
            testPlayer.board.PlaceShips(new Ship("TestShip", 3, new Point(2, 3), 'H'));
            //checks overlap
            Assert.AreEqual(false, testPlayer.board.PlaceShips(new Ship("OverlapShip", 3, new Point(1, 3), 'H')));
            Assert.AreEqual(1, testPlayer.board.ships.Count);
            //checks out of bounds
            Assert.AreEqual(false, testPlayer.board.PlaceShips(new Ship("OutOfBoundsShip", 3, new Point(Settings.boardWidth, Settings.boardWidth), 'H')));
            Assert.AreEqual(1, testPlayer.board.ships.Count);
        }
        //board shootAt() shoots at the board of the player it is called by. 
        //In the game player calls opponent who calls board which calls shootAt()
        [TestMethod]
        public void CanBoardShootAtTile()
        {
            Player testPlayer = new Human("TestHuman", new TestUI());
            testPlayer.board.ShootAt(new Point(2, 3));
            Assert.AreEqual(true, testPlayer.board.yourShots[0] != null);
        }

        [TestMethod]
        public void CanBoardShootAtTileAndRetrieveCorrectInformation()
        {
            Player testPlayer = new Human("TestHuman", new TestUI());
            testPlayer.board.PlaceShips(new Ship("TestShip", 3, new Point(0, 0), 'H'));

            Assert.AreEqual("You missed", testPlayer.board.ShootAt(new Point(4, 4)));

            Assert.AreEqual("You hit a ship", testPlayer.board.ShootAt(new Point(0, 0)));

            testPlayer.board.ShootAt(new Point(1, 0));
            Assert.AreEqual("You sunk TestShip", testPlayer.board.ShootAt(new Point(2, 0)));
        }
    }
}