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
    //Kan ikke få AI til at virke når jeg tester. Af en eller anden grund kan den ikke finde netcore.dll filerne
    //Når vi engang løser det problem skal AI tilføjes til alle disse test's så de ikke længere kun omhandler humans
    [TestClass]
    public class PlayerTest
    {
        [TestMethod]
        public void AreHumansMadeCorrectly()
        {
            TestUI testUI = new TestUI();
            Player[] players = new Player[2];
            players = testUI.InitializePlayers(testUI);
            
            Assert.AreEqual(true, players[0] is Human);
        }
        [TestMethod]
        public void AreHumansConstructedCorrectly()
        {
            Player testPlayer = new Human("TestPlayer", new TestUI());
            AI s = new AI("2");
            s.SetShips();
            s.SetOpponent(testPlayer);

            bool board_expected = true;
            bool board_result = testPlayer.board != null ? true : false;

            bool points_expected = true;
            bool points_result = testPlayer.board != null ? true : false;

            bool playerName_expected = true;
            bool playerName_result = testPlayer.playerName != null ? true : false;

            Assert.AreEqual(board_expected, board_result);
            Assert.AreEqual(points_expected, points_result);
            Assert.AreEqual(playerName_expected, playerName_result);
        }
        [TestMethod]
        public void CanHumanPlaceShipCorrectly()
        {
            Player testPlayer = new Human("TestPlayer", new TestUI());
            testPlayer.SetShips();

            bool expected = true;
            bool result = testPlayer.board.ships != null ? true : false;

            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void DoesHumanPlaceCorrectAmountOfShips()
        {
            Player testPlayer = new Human("TestPlayer", new TestUI());
            testPlayer.SetShips();

            bool expected = true;
            bool result = testPlayer.board.ships.Count == Settings.shipCount ? true : false;

            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void CanHumanSinkAllOpponentsShips()
        {
            TestUI testUI = new TestUI();
            Player testPlayer1 = new Human("TestPlayer1", testUI);
            testPlayer1.SetShips();
            Player testPlayer2 = new Human("TestPlayer2", testUI);
            testPlayer2.SetShips();
            testPlayer1.SetOpponent(testPlayer2);
            //Shoots untill all ships are sunken
            while (testPlayer2.board.sunkenShips != Settings.shipCount)
            {
                testPlayer1.YourTurn();
            }
            Assert.AreEqual(Settings.shipCount, testPlayer2.board.sunkenShips);
        }
    }
}
