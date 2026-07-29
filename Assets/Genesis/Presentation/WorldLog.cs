using System.Collections.Generic;
using System.Text;
using Genesis.Simulation;

namespace Genesis.Presentation
{
    /// <summary>
    /// The append-only file a continuing world writes itself into (L-009; RFC-L003): the world's
    /// file IS its trace. This object owns only the *writing* side — opening and replay stay with
    /// the observer — because writing is where the window can misreport the world's history.
    ///
    /// **One session, one closing mark.** Unity delivers a single closing through two callbacks
    /// (`OnApplicationQuit`, then `OnDestroy`). Before this guard existed, every session
    /// World-001 and World-002 ever lived was written down twice. Replay never noticed — it takes
    /// the largest session mark — but a history that states a fact twice is not a faithful
    /// history, and fidelity is the only thing this file is for.
    ///
    /// The doubled marks already on disk are left exactly where they are. They were really
    /// written; correcting a record silently would be a worse fault than the one being fixed.
    /// </summary>
    public sealed class WorldLog
    {
        private readonly string _path;
        private int _persistedCount;
        private bool _closed;

        public WorldLog(string path, int alreadyPersisted)
        {
            _path = path;
            _persistedCount = alreadyPersisted;
        }

        public string Path => _path;

        /// <summary>Whether this window has already recorded its closing on this world.</summary>
        public bool Closed => _closed;

        /// <summary>
        /// Writes every crossing not yet on file. Append-on-tick: the file never rewrites, only
        /// grows — which is what makes a crash lose nothing but the current tick.
        /// </summary>
        public void AppendCrossings(IReadOnlyList<ExternalEvent> crossings)
        {
            var lines = new StringBuilder();
            for (int i = _persistedCount; i < crossings.Count; i++)
            {
                ExternalEvent crossing = crossings[i];
                lines.Append($"e {crossing.Boundary.Value} {crossing.Target.Kind.Value} {crossing.Target.Place.Value} {crossing.Amount}\n");
            }

            if (lines.Length > 0)
            {
                System.IO.File.AppendAllText(_path, lines.ToString());
                _persistedCount = crossings.Count;
            }
        }

        /// <summary>
        /// Records that the window closed on this world — exactly once, however many times the
        /// host asks. Returns whether this call was the one that wrote.
        /// </summary>
        public bool CloseSession(IReadOnlyList<ExternalEvent> crossings, long tick)
        {
            if (_closed)
            {
                return false;
            }

            _closed = true;
            AppendCrossings(crossings);
            System.IO.File.AppendAllText(_path, $"s {tick}\n");
            return true;
        }

        /// <summary>The ceremony (RFC-L003): abandoning a world is an act the record keeps.</summary>
        public void Abandon(IReadOnlyList<ExternalEvent> crossings, long tick)
        {
            AppendCrossings(crossings);
            System.IO.File.AppendAllText(_path, $"a {tick}\n");
        }
    }
}
