using System;

namespace Domain
{
  public class ManagerTeamHistory
  {public ManagerTeamHistory(Guid managerId, Guid teamId)
  {
      ManagerId=managerId;
      TeamId=teamId;
  }
    public Guid ManagerId { get; private set; }
    public Guid TeamId { get; private set; }
  }
}