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
        public void CanBoardPlaceAShipCorrectly()
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
            // Checks overlap
            Assert.AreEqual(false, testPlayer.board.PlaceShips(new Ship("OverlapShip", 3, new Point(1, 3), 'H')));
            Assert.AreEqual(1, testPlayer.board.ships.Count);
            // Checks out of bounds
            Assert.AreEqual(false, testPlayer.board.PlaceShips(new Ship("OutOfBoundsShip", 3, new Point(Settings.boardWidth, Settings.boardWidth), 'H')));
            Assert.AreEqual(1, testPlayer.board.ships.Count);
        }
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
            testPlayer.board.ShootAt(new Point(1, 0));

            Assert.AreEqual((int)Tile.TileState.missed, testPlayer.board.ShootAt(new Point(4, 4)).tile);
            Assert.AreEqual((int)Tile.TileState.hit, testPlayer.board.ShootAt(new Point(0, 0)).tile); 
            Assert.AreEqual((int)Tile.TileState.sunk, testPlayer.board.ShootAt(new Point(2, 0)).tile);
        }
    }
}