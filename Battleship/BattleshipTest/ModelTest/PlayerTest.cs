using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using BattleshipWeb;
namespace BattleshipTest.ModelTest
{
    [TestClass]
    public class PlayerTest
    {
        [TestMethod]
        public void IsHumanMadeCorrectly()
        {
            TestUI testUI = new TestUI();
            Player[] players = testUI.InitializePlayers(testUI);
            
            Assert.AreEqual(true, players[0] is Human);
        }
        [TestMethod]
        public void IsAIMadeCorrectly()
        {
            TestUI testUI = new TestUI();
            Player[] players = testUI.InitializePlayers(testUI);

            Assert.AreEqual(true, players[1] is AI);
        }
        [TestMethod]
        public void CanHumanPlaceShipsCorrectly()
        {
            Player testHuman = new Human("TestHuman", new TestUI());

            testHuman.SetShips();

            Assert.AreEqual(Settings.shipCount, testHuman.board.ships.Count);
        }
        [TestMethod]
        public void CanAIPlaceShipsCorrectly()
        {
            Player testAI = new Human("TestAI", new TestUI());

            testAI.SetShips();

            Assert.AreEqual(Settings.shipCount, testAI.board.ships.Count);
        }
        [TestMethod]
        public void CanHumanSinkAllOpponentsShips()
        {
            TestUI testUI = new TestUI();
            Player testPlayer1 = new Human("TestPlayer1", testUI);
            Player testPlayer2 = new Human("TestPlayer2", testUI);

            testPlayer1.SetShips();
            testPlayer2.SetShips();
            testPlayer1.SetOpponent(testPlayer2);
            //Shoots untill all ships are sunken
            while (testPlayer2.board.sunkenShips != Settings.shipCount)
            {
                testPlayer1.YourTurn();
            }
            Assert.AreEqual(Settings.shipCount, testPlayer2.board.sunkenShips);
        }
        [TestMethod]
        public void CanHumansShootOpponent()
        {
            TestUI testUI = new TestUI();
            Player testHuman1 = new Human("TestHuman1", testUI);
            Player testHuman2 = new Human("TestHuman2", testUI);

            testHuman1.SetShips();
            testHuman2.SetShips();
            testHuman1.SetOpponent(testHuman2);
            testHuman1.YourTurn();

            Assert.AreEqual(1, testHuman2.board.yourShots.Count);
        }
    }
}
