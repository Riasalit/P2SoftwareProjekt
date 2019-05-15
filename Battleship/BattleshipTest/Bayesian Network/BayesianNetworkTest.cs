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

namespace BattleshipTest.BayesianNetworkTest
{
    [TestClass]
    public class BayesianNetworkTest
    {
        [TestMethod]
        public void CanAIShootIfShipsArePlacedBesideEachOther()
        {
            Player testHuman = new AI("TestHuman");
            Player testAI = new AI("TestAI");
            int i = 0;

            foreach (KeyValuePair<string, int> ship in Settings.ships)
            {
                testHuman.board.PlaceShips(new Ship(ship.Key, ship.Value, new Point(i++, 0), 'V'));
            }
            testAI.SetOpponent(testHuman);

            while (testHuman.board.sunkenShips != Settings.shipCount)
            {
                testAI.YourTurn();
            }

            Assert.AreEqual(Settings.shipCount, testHuman.board.sunkenShips);
        }

    }
}
