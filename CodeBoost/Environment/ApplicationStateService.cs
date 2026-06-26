namespace CodeBoost.Environment;

public static class ApplicationStateService
{
    /// <summary>
    /// Used to access debug logic via branching rather than preprocessor defines.
    /// </summary>
    #if DEBUG
    public const bool IsDebug = true;
    #else
    public const bool IsDebug = false;
    #endif

    /// <summary>
    /// Invoked when the ApplicationState is set.
    /// </summary>
    internal static event ApplicationStateSetEventHandler ApplicationStateSet;

    internal delegate void ApplicationStateSetEventHandler(IApplicationState previousApplicationState, IApplicationState nextApplicationState);

    /// <summary>
    /// The IApplicationState to use.
    /// </summary>
    internal static IApplicationState ApplicationState;

    /// <summary>
    /// The message used when trying to access the ApplicationState when there is not an instance created.
    /// </summary>
    private static readonly string ApplicationStateIsNullMessage = $"[{nameof(ApplicationState)}] is null. Use [{nameof(ApplicationStateService)}] to set a service.";
        
    static ApplicationStateService()
    {
        UseApplicationState(new IdeApplicationState());
    }

    /// <summary>
    /// Specifies which IApplicationState to use.
    /// </summary>
    public static void UseApplicationState(IApplicationState applicationState)
    {
        IApplicationState previousApplicationState = ApplicationState;
        ApplicationState = applicationState;
            
        ApplicationStateSet?.Invoke(previousApplicationState, applicationState);
    }

    /// <summary>
    /// Returns whether the application is quitting.
    /// </summary>
    /// <returns>True if the application is quitting; otherwise false.</returns>
    internal static bool IsQuitting()
    {
        if (ApplicationState is not null)
            return ApplicationState.IsQuitting();

        throw new(ApplicationStateIsNullMessage);
    }

    /// <summary>
    /// Returns whether the application is playing.
    /// </summary>
    /// <returns>True if the application is playing; otherwise false.</returns>
    internal static bool IsPlaying()
    {
        if (ApplicationState is not null)
            return ApplicationState.IsPlaying();

        throw new(ApplicationStateIsNullMessage);
    }

    /// <summary>
    /// Quits the application for the editor or a build.
    /// </summary>
    internal static void Quit()
    {
        if (ApplicationState is null)
            throw new(ApplicationStateIsNullMessage);

        ApplicationState.Quit();
    }

    /// <summary>
    /// Returns whether the application is being run within an editor.
    /// </summary>
    /// <returns>True if the application is being run within an editor; otherwise false.</returns>
    public static bool IsEditor()
    {
        if (ApplicationState is not null)
            return ApplicationState.IsEditor();

        throw new(ApplicationStateIsNullMessage);
    }

    /// <summary>
    /// Returns whether the application is a build with development or debugging enabled.
    /// </summary>
    /// <returns>True if the application is a development or debug build; otherwise false.</returns>
    public static bool IsDevelopmentBuild()
    {
        if (ApplicationState is not null)
            return ApplicationState.IsDevelopment();

        throw new(ApplicationStateIsNullMessage);
    }

    /// <summary>
    /// Returns whether the application is a GUI build, such as a client build.
    /// </summary>
    /// <returns>True if the application is a GUI build; otherwise false.</returns>
    public static bool IsGuiBuild()
    {
        if (ApplicationState is not null)
            return ApplicationState.IsGuiBuild();

        throw new(ApplicationStateIsNullMessage);
    }

    /// <summary>
    /// Returns whether the application is a headless build, such as a server build.
    /// </summary>
    /// <returns>True if the application is a headless build; otherwise false.</returns>
    public static bool IsHeadlessBuild()
    {
        if (ApplicationState is not null)
            return ApplicationState.IsHeadlessBuild();

        throw new(ApplicationStateIsNullMessage);
    }

}