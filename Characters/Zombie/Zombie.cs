using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static Constants;

public partial class Zombie : NPC
{
  	public override void _Ready()
  	{
		try
		{
			base._Ready();
			//SetPhysicsProcess(false);
		}
		finally
		{
			ListNullMembersAfter<Zombie>(1.0f);
		}
  	}
}
