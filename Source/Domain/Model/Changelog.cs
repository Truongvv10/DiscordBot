using Domain.Enums;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model {
    public class Changelog {
        #region Private Properties
        private int? _id;
        private string _name;
        private string _version;
        private ulong? channelPostId;
        private ulong? channelSetupId;
        private HashSet<ulong> messageIds = new();
        private Dictionary<string, List<string>> _changes = new();
        private Dictionary<string, Changelog> _logs = new();
        #endregion

        #region Constructors
        public Changelog() {
            _version = "1.0";
        }
        public Changelog(string name) : this() {
            _name = name;
        }
        public Changelog(int id, string name) : this(name) {
            _id = id;
        }
        #endregion

        #region Getter & Setter
        public int Id { get; set; }
        public string Name {
            get => _name;
            set { if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"Changelog.Name: \"{value}\" can not be empty or null"); else _name = value.ToUpper().Trim(); }
        }
        public string Version {
            get => _version;
            set { if (string.IsNullOrWhiteSpace(value) && double.TryParse(value, out double version)) throw new DomainException($"Changelog.Version: \"{value}\" can not be negative or 0"); else _version = value; }
        }
        public IReadOnlyDictionary<string, List<string>> Changes {
            get => _changes;
        }
        public IReadOnlyDictionary<string, Changelog> Logs {
            get => _logs;
        }
        #endregion

        #region Methods
        public void AddChangelog(string title, string description) {
            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(description)) {
                _version = IncrementVersion(_version, Versioning.MINOR);
                if (_changes.ContainsKey(title)) {
                    _changes[title].Add(description);
                } else {
                    _changes[title] = new List<string> { description };
                }
                _changes[title].Sort();
            } else throw new DomainException($"$\"Changelog.AddChangelog: title: \"{title}\" & description: \"{description}\" can not be empty or null");
        }
        public Changelog UpdateChangelog() {
            Changelog changelog = this;
            LogChangelog(changelog);
            _changes.Clear();
            _version = IncrementVersion(_version, Versioning.MAJOR);
            return changelog;
        }
        private void LogChangelog(Changelog changelog) {
            if (changelog is not null) {
                if (!_logs.ContainsKey(_version)) {
                    _logs.Add(_version, changelog);
                }
            } else throw new DomainException($"$\"Changelog.UpdateChangelog: \"{changelog.Name}\" can not be null");
        }
        private string IncrementVersion(string version, Versioning versionType) {
            var parts = version.Split('.');
            if (parts.Length != 2) {
                throw new FormatException($"Changelog.IncrementVersion: Version \"{version}\" must be in the format 'major.minor'");
            }
            if (!int.TryParse(parts[0], out int major) || !int.TryParse(parts[1], out int minor)) {
                throw new FormatException($"Changelog.IncrementVersion: Major and minor versions must be integers, got input: \"{version}\"");
            }
            if (versionType == Versioning.MAJOR) major++;
            if (versionType == Versioning.MINOR) minor++;
            return $"{major}.{minor}";
        }
        #endregion
    }
}
