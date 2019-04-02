using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BattleshipTest
{
    [TestClass]
    public class RunningGameTest
    {
        [TestMethod]
        public void CanGameMakeAIPlayer()
        {
            Player player = new AI("Erik");

            Assert.IsNotNull(player);
        }
        [TestMethod]
        public void CanHumanSetShip() // IF GetShip changes in TestUI, then change this test
        {
            IUserInterface ui = new TestUI();
            Player player = new Human("Liv", ui);

            player.SetShips();

            Assert.IsTrue(player.board.ships.Count > 0);
        }
        [TestMethod]
        public void CanAISetShip()
        {
            IUserInterface ui = new TestUI();
            Player player = new AI("jakob");

            player.SetShips();

            Assert.IsTrue(player.board.ships.Count > 0);
        }
        [TestMethod]
        public void CannotPlaceShipThatOverlapWithOtherShip()
        {
            Player player = new AI("Erik");
            Ship testship = new Ship("Testship", 3, 0, 0, 'V');
            Ship overlapship = new Ship("Overlapship", 3, 0, 0, 'H');
            bool expected, test;

            player.board.PlaceShips(testship);
            test = player.board.PlaceShips(overlapship);

            Assert.AreEqual(false, test);
        }
        [TestMethod]
        public void CannotPlaceShipThatIsOutOfBoardBounds()
        {
            Player player = new AI("Erik");
            Ship testship = new Ship("Testship", 3, 8, 0, 'H');
            bool test;

            test = player.board.PlaceShips(testship);

            Assert.AreEqual(false, test);
        }
        [TestMethod]
        public void ReturnsHitIfHitShip()
        {
            Player player = new AI("Erik");
            Ship testship = new Ship("Testship", 3, 0, 0, 'V');
            string test;

            player.board.PlaceShips(testship);
            test = player.board.ShootAt(0, 0);

            Assert.AreEqual("You hit a ship", test);
        }
        [TestMethod]
        public void ReturnsSunkenIfHitOnShipIsSunken()
        {
            Player player = new AI("Erik");
            Ship testship = new Ship("Testship", 2, 0, 0, 'V');

            
            string test;

            player.board.PlaceShips(testship);
            player.board.ShootAt(0, 0);
            test = player.board.ShootAt(0, 1);

            Assert.AreEqual("You sunk Testship!", test);
        }
        [TestMethod]
        public void ReturnsMissedIfShipIsNotHit()
        {
            Player player = new AI("Erik");
            Ship testship = new Ship("Testship", 2, 0, 0, 'V');
            string test;

            player.board.PlaceShips(testship);
            test = player.board.ShootAt(6, 1);

            Assert.AreEqual("You missed", test);
        }
        [TestMethod]
        public void ReturnsAlreadyHitIfTileIsAlreadyHit()
        {
            Player player = new AI("Erik");
            string test;

            player.board.ShootAt(6, 1);
            test = player.board.ShootAt(6, 1);


            Assert.AreEqual("Already hit", test);
        }


    }
}
