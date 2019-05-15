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
            Point point = new Point(2, 3);

            testPlayer.board.PlaceShips(new Ship("TestShip", 3, point, 'H'));

            Assert.AreEqual(point, testPlayer.board.ships[0].shipCoord);
        }
        [TestMethod]
        public void CanBoardPlaceShipsWithCorrectOrientation()
        {
            Player testPlayer = new Human("TestHuman", new TestUI());
            Point point = new Point(2, 3);

            testPlayer.board.PlaceShips(new Ship("TestShip", 3, point, 'H'));
            Tile hitTile = testPlayer.board.ShootAt(new Point(4, 3));
            Tile missTile = testPlayer.board.ShootAt(new Point(5, 3));

            Assert.AreEqual(true, hitTile.tile == (int) Tile.TileState.hit && missTile.tile == (int) Tile.TileState.missed);
        }
        [TestMethod]
        public void CanBoardAvoidOverlapCorrectly()
        {
            Player testPlayer = new Human("TestHuman", new TestUI());

            testPlayer.board.PlaceShips(new Ship("TestShip", 3, new Point(2, 3), 'H'));
            testPlayer.board.PlaceShips(new Ship("OverlapShip", 3, new Point(1, 3), 'H'));
            // Checks overlap
            Assert.AreEqual(1, testPlayer.board.ships.Count);
        }
        [TestMethod]
        public void CanBoardAvoidPlacingOutOfBoundsCorrectly()
        {
            Player testPlayer = new Human("TestHuman", new TestUI());

            testPlayer.board.PlaceShips(new Ship("TestShip", 3, new Point(2, 3), 'H'));
            testPlayer.board.PlaceShips(new Ship("OutOfBoundsShip", 3, new Point(Settings.boardWidth, Settings.boardWidth), 'H'));
            // Checks if out of bound
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
        public void CanBoardShootAtTileAndRetrieveCorrectMissedInformation()
        {
            Player testPlayer = new Human("TestHuman", new TestUI());

            testPlayer.board.PlaceShips(new Ship("TestShip", 3, new Point(0, 0), 'H'));

            Assert.AreEqual((int)Tile.TileState.missed, testPlayer.board.ShootAt(new Point(4, 4)).tile);
        }
        [TestMethod]
        public void CanBoardShootAtTileAndRetrieveCorrectHitInformation()
        {
            Player testPlayer = new Human("TestHuman", new TestUI());

            testPlayer.board.PlaceShips(new Ship("TestShip", 3, new Point(0, 0), 'H'));
            
            Assert.AreEqual((int)Tile.TileState.hit, testPlayer.board.ShootAt(new Point(0, 0)).tile);
        }
        [TestMethod]
        public void CanBoardShootAtTileAndRetrieveCorrectSunkInformation()
        {
            Player testPlayer = new Human("TestHuman", new TestUI());

            testPlayer.board.PlaceShips(new Ship("TestShip", 3, new Point(0, 0), 'H'));
            testPlayer.board.ShootAt(new Point(0, 0));
            testPlayer.board.ShootAt(new Point(1, 0));

            Assert.AreEqual((int)Tile.TileState.sunk, testPlayer.board.ShootAt(new Point(2, 0)).tile);
        }
    }
}