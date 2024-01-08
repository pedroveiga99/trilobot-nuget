namespace Trilobot.Controllers;
public class UnderlightController : Sn3218Controller
{
    readonly double[] Underlights;

    public UnderlightController() : base()
    {
        Underlights = new double[18];

        Reset();
        Output(Underlights);

        EnableLeds();
        Disable();
    }

    /// <summary>
    /// Shows the previously stored colors on Trilobot's underlights
    /// </summary>
    public void ShowUnderlight()
    {
        Output(Underlights);
        Enable();
    }

    /// <summary>
    /// Disables Trilobot's underlighting, preserving the last set colors
    /// </summary>
    public void DisableUnderlight()
    {
        Disable();
    }

    /// <summary>
    /// Sets a single underlight to a given RGB color
    /// </summary>
    /// <param name="light">ID of the light to set the color of</param>
    /// <param name="r">Red component of the color</param>
    /// <param name="g">Green component of the color</param>
    /// <param name="b">Blue component of the color</param>
    /// <param name="show">Whether or not to show the new color immediately</param>
    public void SetUnderlight(TrilobotLed light, int r, int g, int b, bool show = true)
    {
        if (r < 0 || r > 255)
            throw new ArgumentOutOfRangeException(r.ToString(), "Red component must be between 0 and 255");
        if (g < 0 || g > 255)
            throw new ArgumentOutOfRangeException(g.ToString(), "Green component must be between 0 and 255");
        if (b < 0 || b > 255)
            throw new ArgumentOutOfRangeException(b.ToString(), "Blue component must be between 0 and 255");

        int lightId = (int)light * 3;

        Underlights[lightId] = Convert.ToDouble(r);
        Underlights[lightId + 1] = Convert.ToDouble(g);
        Underlights[lightId + 2] = Convert.ToDouble(b);

        if(show)
            ShowUnderlight();
    }

    /// <summary>
    /// Fill all the underlights with a given RGB color
    /// </summary>
    /// <param name="r">Red component of the color</param>
    /// <param name="g">Green component of the color</param>
    /// <param name="b">Blue component of the color</param>
    /// <param name="show">Whether or not to show the new color immediately</param>
    public void FillUnderlight(int r, int g, int b, bool show = true)
    {
        foreach (TrilobotLed led in Enum.GetValues(typeof(TrilobotLed)))
        {
            SetUnderlight(led, r, g, b, false);
        }

        if(show)
            ShowUnderlight();
    }

    /// <summary>
    /// Clear the color of a single underlight. This has the effect of turning it off
    /// </summary>
    /// <param name="light">ID of the light to clear</param>
    /// <param name="show">Whether or not to show the new color immediately</param>
    public void ClearSingleUnderlight(TrilobotLed light, bool show = true)
    {
        SetUnderlight(light, 0, 0, 0, show);
    }

    /// <summary>
    /// Clear the color of all underlights. This has the effect of turning them off
    /// </summary>
    /// <param name="show">Whether or not to show the new color immediately</param>
    public void ClearAllUnderlight(bool show = true)
    {
        FillUnderlight(0, 0, 0, show);
    }

    public async Task TestUnderlight(TimeSpan interval)
    {
        FillUnderlight(127, 127, 127);
        await Task.Delay(interval);
        ClearAllUnderlight();
    }

    public new void Dispose()
    {
        ClearAllUnderlight();
        Disable();
        base.Dispose();
    }
}
