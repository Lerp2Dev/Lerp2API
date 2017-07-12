/// <summary>
/// Delegate Callback
/// </summary>
public delegate void Callback();

/// <summary>
/// Delegate Callback
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="arg1">The arg1.</param>
public delegate void Callback<T>(T arg1);

/// <summary>
/// Delegate Callback
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="U"></typeparam>
/// <param name="arg1">The arg1.</param>
/// <param name="arg2">The arg2.</param>
public delegate void Callback<T, U>(T arg1, U arg2);

/// <summary>
/// Delegate Callback
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="U"></typeparam>
/// <typeparam name="V"></typeparam>
/// <param name="arg1">The arg1.</param>
/// <param name="arg2">The arg2.</param>
/// <param name="arg3">The arg3.</param>
public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);