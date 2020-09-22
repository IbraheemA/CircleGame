using System;
using System.Collections.Generic;

public class CommandQueue {

	Queue<Action<Entity>> queue = new Queue<Action<Entity>>();

	public CommandQueue(){

	}

	public void Execute(Entity entity){
		while(queue.Count > 0){
			Action<Entity> command = queue.Dequeue();
			command.Invoke(entity);
		}
	}

	public void Clear(){
		queue.Clear();
	}

	public void Issue(Action<Entity> command){
		queue.Enqueue(command);
	}

}
