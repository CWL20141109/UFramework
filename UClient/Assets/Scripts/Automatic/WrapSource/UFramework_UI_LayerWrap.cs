﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class UFramework_UI_LayerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginEnum(typeof(UFramework.UI.Layer));
		L.RegVar("SCNEN", get_SCNEN, null);
		L.RegVar("PANEL", get_PANEL, null);
		L.RegVar("MESSAGE_BOX", get_MESSAGE_BOX, null);
		L.RegVar("GUIDE", get_GUIDE, null);
		L.RegVar("NOTIFICATION", get_NOTIFICATION, null);
		L.RegVar("NETWORK", get_NETWORK, null);
		L.RegVar("LOADER", get_LOADER, null);
		L.RegVar("TOP", get_TOP, null);
		L.RegFunction("IntToEnum", IntToEnum);
		L.EndEnum();
		TypeTraits<UFramework.UI.Layer>.Check = CheckType;
		StackTraits<UFramework.UI.Layer>.Push = Push;
	}

	static void Push(IntPtr L, UFramework.UI.Layer arg)
	{
		ToLua.Push(L, arg);
	}

	static bool CheckType(IntPtr L, int pos)
	{
		return TypeChecker.CheckEnumType(typeof(UFramework.UI.Layer), L, pos);
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_SCNEN(IntPtr L)
	{
		ToLua.Push(L, UFramework.UI.Layer.SCNEN);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_PANEL(IntPtr L)
	{
		ToLua.Push(L, UFramework.UI.Layer.PANEL);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_MESSAGE_BOX(IntPtr L)
	{
		ToLua.Push(L, UFramework.UI.Layer.MESSAGE_BOX);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_GUIDE(IntPtr L)
	{
		ToLua.Push(L, UFramework.UI.Layer.GUIDE);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_NOTIFICATION(IntPtr L)
	{
		ToLua.Push(L, UFramework.UI.Layer.NOTIFICATION);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_NETWORK(IntPtr L)
	{
		ToLua.Push(L, UFramework.UI.Layer.NETWORK);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_LOADER(IntPtr L)
	{
		ToLua.Push(L, UFramework.UI.Layer.LOADER);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_TOP(IntPtr L)
	{
		ToLua.Push(L, UFramework.UI.Layer.TOP);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		UFramework.UI.Layer o = (UFramework.UI.Layer)arg0;
		ToLua.Push(L, o);
		return 1;
	}
}

