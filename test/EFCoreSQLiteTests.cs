using System;
using System.Drawing;
using System.Linq;
using Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace test {
    public class EFCoreSQLiteTests {
        //no need for SQLite ref in this project. The data project uses it by default.
        private static Team CreateTeamAjax () {
            return new Team ("AFC Ajax", "The Lancers", "1900", "Amsterdam Arena");
        }

        [Fact]
        public void CanStoreAndMaterializeImmutableTeamNameFromDataStore () {
            var team = CreateTeamAjax ();
            using (var context = new TeamContext ()) {
                context.Database.EnsureDeleted ();
                context.Database.EnsureCreated ();
                context.Teams.Add (team);
                context.SaveChanges ();
            }
            using (var context = new TeamContext ()) {
                var storedTeam = context.Teams.FirstOrDefault ();
                Assert.Equal ("AFC Ajax", storedTeam.TeamName);
            }
        }

        [Fact]
        public void CanStoreAndRetrievePlayerName () {
            var team = CreateTeamAjax ();
            team.AddPlayer ("André", "Onana", out string response);

            using (var context = new TeamContext ()) {
                context.Database.EnsureDeleted ();
                context.Database.EnsureCreated ();
                context.Teams.Add (team);
                context.SaveChanges ();
            }
            using (var context = new TeamContext ()) {
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

            using (var context = new TeamContext ()) {
                context.Database.EnsureDeleted ();
                context.Database.EnsureCreated ();
                context.Teams.Add (team);
                context.SaveChanges ();
            }
            using (var context = new TeamContext ()) {
                var storedTeam = context.Teams.Include (t => t.Players).FirstOrDefault ();
                Assert.Single (storedTeam.Players);
            }
        }

        [Fact]
        public void TeamPreventsAddingPlayersToExistingTeamWhenPlayersNotInMemory () {
            var team = CreateTeamAjax ();
            team.AddPlayer ("André", "Onana", out string response);

            using (var context = new TeamContext ()) {
                context.Database.EnsureDeleted ();
                context.Database.EnsureCreated ();
                context.Teams.Add (team);
                context.SaveChanges ();
            }
            using (var context = new TeamContext ()) {
                var storedTeam = context.Teams.FirstOrDefault ();
                storedTeam.AddPlayer ("Matthijs", "de Ligt", out response);
                Assert.Equal ("You must first retrieve", response.Substring (0, 23));
            }
        }

        [Fact]
        public void TeamAllowsAddingPlayersToExistingTeamWhenPlayersAreLoaded () {
            var team = CreateTeamAjax ();
            team.AddPlayer ("André", "Onana", out string response);

            using (var context = new TeamContext ()) {
                context.Database.EnsureDeleted ();
                context.Database.EnsureCreated ();
                context.Teams.Add (team);
                context.SaveChanges ();
            }
            using (var context = new TeamContext ()) {
                var storedTeam = context.Teams.Include (t => t.Players).ThenInclude (p => p.NameFactory).FirstOrDefault ();
                storedTeam.AddPlayer ("Matthijs", "de Ligt", out response);
                Assert.Equal (2, storedTeam.Players.Count ());
            }
        }

        [Fact]
        public void CanStoreAndRetrieveManagerTeamHistory () {
            var team = CreateTeamAjax ();
            team.AddPlayer ("André", "Onana", out string response);
            var firstmanager = new Manager ("Marcel", "Keizer");
            team.ChangeManagement (firstmanager);
            var secondmanager = new Manager ("Erik", "ten Hag");
            team.ChangeManagement (secondmanager);
            team.ChangeManagement (new Manager ("Christian", "Weyer"));

            using (var context = new TeamContext ()) {
                context.Database.EnsureDeleted ();
                context.Database.EnsureCreated ();
                context.AddRange (team, firstmanager, secondmanager);
                context.SaveChanges ();
            }
            using (var context = new TeamContext ()) {
                var managers = context.Managers.Include (m => m.PastTeams).ToList ();
                Assert.Equal (3, managers.Count);
                foreach (var manager in managers) 
                { 
                    //will display when debugging
                    Console.WriteLine ($"{manager.Name}: {manager.PastTeams.Count} Past Teams");
                }
            }

        }

    }
}