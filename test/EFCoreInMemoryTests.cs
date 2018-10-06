using System;
using System.Drawing;
using System.Linq;
using Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace test {
    public class EFCoreInMemoryTests {

        private static Team CreateTeamAjax () {
            return new Team ("AFC Ajax", "The Lancers", "1900", "Amsterdam Arena");
        }

        [Fact]
        public void CanStoreAndMaterializeImmutableTeamNameFromDataStore () {
            var team = CreateTeamAjax ();
            var options = new DbContextOptionsBuilder<TeamContext> ().UseInMemoryDatabase ("immutableTeamName").Options;
            using (var context = new TeamContext (options)) {
                context.Teams.Add (team);
                context.SaveChanges ();
            }
            using (var context = new TeamContext (options)) {
                var storedTeam = context.Teams.FirstOrDefault ();
                Assert.Equal ("AFC Ajax", storedTeam.TeamName);
            }
        }

        [Fact]
        public void CanStoreAndRetrievePlayerName () {
            var team = CreateTeamAjax ();
            team.AddPlayer ("André", "Onana", out string response);

            var options = new DbContextOptionsBuilder<TeamContext> ().UseInMemoryDatabase ("playername").Options;
            using (var context = new TeamContext (options)) {
                context.Teams.Add (team);
                context.SaveChanges ();
            }
            using (var context = new TeamContext (options)) {
                //note a current bug:
                //https://github.com/aspnet/EntityFrameworkCore/issues/9210
                //requires a workaround of including the owned entity of an included navigation property
                var storedTeam = context.Teams.Include (t => t.Players).ThenInclude (p => p.NameFactory).FirstOrDefault ();
                Assert.Single (storedTeam.Players);
                Assert.Equal ("André Onana", storedTeam.Players.First ().Name);

            }
        }

        [Fact]
        public void CanStoreAndRetrieveTeamPlayers () {
            var team = CreateTeamAjax ();
            team.AddPlayer ("André", "Onana", out string response);

            var options = new DbContextOptionsBuilder<TeamContext> ().UseInMemoryDatabase ("storeretrieveplayer").Options;
            using (var context = new TeamContext (options)) {
                context.Teams.Add (team);
                context.SaveChanges ();
            }
            using (var context = new TeamContext (options)) {
                var storedTeam = context.Teams.Include (t => t.Players).FirstOrDefault ();
                Assert.Single (storedTeam.Players);
            }
        }

        [Fact]
        public void TeamPreventsAddingPlayersToExistingTeamWhenPlayersNotInMemory () {
            var team = CreateTeamAjax ();
            team.AddPlayer ("André", "Onana", out string response);

            var options = new DbContextOptionsBuilder<TeamContext> ().UseInMemoryDatabase ("preventplayeronteamwithplayersnotloaded").Options;
            using (var context = new TeamContext (options)) {
                context.Teams.Add (team);
                context.SaveChanges ();
            }
            using (var context = new TeamContext (options)) {
                var storedTeam = context.Teams.FirstOrDefault ();
                storedTeam.AddPlayer ("Matthijs", "de Ligt", out response);
                Assert.Equal ("You must first retrieve", response.Substring (0, 23));
            }
        }

        [Fact]
        public void TeamAllowsAddingPlayersToExistingTeamWhenPlayersAreLoaded () {
            var team = CreateTeamAjax ();
            team.AddPlayer ("André", "Onana", out string response);

            var options = new DbContextOptionsBuilder<TeamContext> ().UseInMemoryDatabase ("allowplayeronteamwithplayersloaded").Options;
            using (var context = new TeamContext (options)) {
                context.Teams.Add (team);
                context.SaveChanges ();
            }
            using (var context = new TeamContext (options)) {
                var storedTeam = context.Teams.Include (t => t.Players).ThenInclude (p => p.NameFactory).FirstOrDefault ();
                storedTeam.AddPlayer ("Matthijs", "de Ligt", out response);
                Assert.Equal (2, storedTeam.Players.Count ());
            }
        }

        [Fact]
        public void CanStoreAndRetrieveManagerTeamHistory () {
            var options = new DbContextOptionsBuilder<TeamContext> ().UseInMemoryDatabase ("CanStoreAndRetrieveManagerTeamHistory").Options;
            var team = CreateTeamAjax ();
            team.AddPlayer ("André", "Onana", out string response);
            var firstmanager = new Manager ("Marcel", "Keizer");
            team.ChangeManagement (firstmanager);
            var secondmanager = new Manager ("Erik", "ten Hag");
            team.ChangeManagement (secondmanager);
            team.ChangeManagement (new Manager ("Christian", "Weyer"));

            using (var context = new TeamContext (options)) {
                context.Database.EnsureDeleted ();
                context.Database.EnsureCreated ();
                context.AddRange (team, firstmanager, secondmanager);
                context.SaveChanges ();
            }
            using (var context = new TeamContext (options)) {
                var managers = context.Managers.Include (m => m.PastTeams).ToList ();
                Assert.Equal (3, managers.Count);
                foreach (var manager in managers) {
                    //will display when debugging
                    Console.WriteLine ($"{manager.Name}: {manager.PastTeams.Count} Past Teams");
                }
            }

        }
    }
}