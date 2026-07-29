using System;
using System.IO;
using Genesis.Presentation;
using Genesis.Simulation;
using NUnit.Framework;

namespace Genesis.Tests
{
    /// <summary>
    /// The fidelity of a continuing world's history. Every session World-001 and World-002 ever
    /// lived was written down twice, because Unity delivers one closing through two callbacks.
    /// Replay never noticed; the record did. These tests hold the repair — and they run on a
    /// throwaway file, never on a living world.
    /// </summary>
    public sealed class WorldLogTests
    {
        private string _path;

        [SetUp]
        public void SetUp()
        {
            _path = Path.Combine(Path.GetTempPath(), $"genesis-worldlog-{Guid.NewGuid():N}.log");
            File.WriteAllText(_path, "# a world that exists only for this test\n");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
            }
        }

        [Test]
        public void A_Session_Lived_Once_Is_Recorded_Once_However_Often_It_Is_Closed()
        {
            var log = new WorldLog(_path, 0);
            var none = new ExternalEvent[0];

            Assert.IsTrue(log.CloseSession(none, 428), "the first closing is the one that writes");
            Assert.IsFalse(log.CloseSession(none, 428), "a second closing writes nothing");
            log.CloseSession(none, 428);

            Assert.AreEqual(1, CountLinesStartingWith("s "), "exactly one session mark");
            Assert.IsTrue(log.Closed);
        }

        [Test]
        public void Crossings_Reach_The_File_Exactly_Once()
        {
            var log = new WorldLog(_path, 0);
            var crossings = new[]
            {
                new ExternalEvent(new Tick(3), new Cell(new Place(100), new Kind(11)), 1),
                new ExternalEvent(new Tick(7), new Cell(new Place(200), new Kind(12)), 1),
            };

            log.AppendCrossings(crossings);
            log.AppendCrossings(crossings); // append-on-tick runs every tick; nothing may repeat
            log.CloseSession(crossings, 9);

            Assert.AreEqual(2, CountLinesStartingWith("e "));
            Assert.AreEqual(1, CountLinesStartingWith("s "));
        }

        [Test]
        public void Abandonment_Is_Recorded_And_Is_Not_A_Session_Mark()
        {
            var log = new WorldLog(_path, 0);
            var none = new ExternalEvent[0];

            log.Abandon(none, 340);

            Assert.AreEqual(1, CountLinesStartingWith("a "));
            Assert.AreEqual(0, CountLinesStartingWith("s "), "the ceremony is not a closing");
        }

        private int CountLinesStartingWith(string prefix)
        {
            int count = 0;
            foreach (string line in File.ReadAllLines(_path))
            {
                if (line.StartsWith(prefix, StringComparison.Ordinal))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
