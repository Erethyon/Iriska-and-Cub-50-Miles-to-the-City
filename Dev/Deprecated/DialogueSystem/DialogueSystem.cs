using System;
using System.Collections.Generic;

class Sentence{
	// Содержит в себе
	Question question;
	List<Answer>? answers;
}

class Question{
	uint id;
	string text;
}

class Answer{
	private uint id;
	private uint incomingId;
	private string text;
	
	private Action action;
	
	public Answer(Action action)
	{
		this.action = action;
	}
	
	public void OnAnswerChosen(){
		action();
	}
}