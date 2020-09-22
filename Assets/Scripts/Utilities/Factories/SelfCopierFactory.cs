using System;
using System.Collections.Generic;

static class SelfTypeCreatorFactory<T> where T : class, ISelfTypeCreator {

	static Dictionary<Type, T> _instanceDict = new Dictionary<Type, T>();

	public static T2 Create<T2>() where T2 : class, T, new() {
		if(!_instanceDict.TryGetValue(typeof(T2), out T c)){
			c = _instanceDict[typeof(T2)] = new T2();
		}
		else {
			c = c.CreateSelfType() as T;
		}
		return c as T2;
	}
}