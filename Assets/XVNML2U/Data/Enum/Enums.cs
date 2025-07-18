namespace XVNML2U
{
    public enum CastMotionType
    {
        Instant,
        Interpolation
    }

    public enum Anchoring
    {
        Left = -1,
        Center,
        Right
    }

    public enum LoadingMode
    {
        Internal,
        External
    }

    public enum CastGraphicMode
    {
        Image,
        Sprite,
        Live2D,
        Other
    }

    public enum EnterSide
    {
        Left,
        Right
    }

    public enum TransitionMode
    {
        Instant,
        FadeIn,
        FadeInFromLeft,
        FadeInFromRight,
        FadeInFromTop,
        FadeInFromBottom,
        FadeOutToLeft,
        FadeOutToRight,
        FadeOutToTop,
        FadeOutToBottom,
        FadeOut
    }

    public enum ElementReferenceValueType
    {
        ID,
        Name
    }

    public enum ControlState : sbyte
    {
        None,
        Bold,
        Italize,
        Underline,
        StrikeThrough,
        Color,
        TypeFace,
        Size,
        Style,
        Sprite
    }
}