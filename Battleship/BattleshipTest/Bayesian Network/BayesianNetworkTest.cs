using BattleshipWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BattleshipTest.BayesianNetworkTest
{
    [TestClass]
    public class BayesianNetworkTest
    {
        [TestMethod]
        public void CanAISetShipsCorrectly()
        {
            Player testAI = new AI("TestAI");

            testAI.SetShips();

            Assert.AreEqual(Settings.shipCount, testAI.board.ships.Count);
        }
    }
}
