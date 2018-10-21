using System;
using SharedKernel;

public class Player {
  private Player () { }
  public Player (string firstname, string lastname, PlayerPosition playerPosition) {
    NameFactory = PersonFullName.Create (firstname, lastname);
    Id = Guid.NewGuid ();
    Position = playerPosition;
  }
  public Guid Id { get; private set; }
  public PersonFullName NameFactory { get; set; }
  public string Name => NameFactory.FullName;

  public PlayerPosition Position { get; private set; }
  public void ChangePosition (PlayerPosition newPosition) {
    Position = newPosition;
  }
}