using XVNML.Core.Tags;
using XVNML.Utilities.Tags;

namespace XVNML2U
{
    [AssociateWithTag("struct", typeof(Proxy), TagOccurance.Multiple, true)]
    public sealed class StructTag : UserDefined
    {
        internal string structName => TagName;
        internal object this[string name]
        {
            get
            {
                return GetElement<PropertyTag>(name).propertyValue;
            }
            set
            {
                GetElement<PropertyTag>(name).propertyValue = value;
            }
        }

        public override void OnResolve(string fileOrigin)
        {
            base.OnResolve(fileOrigin);
        }
    }

    [AssociateWithTag("property", typeof(StructTag), TagOccurance.Multiple, true)]
    public sealed class PropertyTag : UserDefined
    {
        internal string propertyName => TagName;
        internal object propertyValue;

        public override void OnResolve(string fileOrigin)
        {
            AllowedParameters = new[]
            {
                "value"
            };

            AllowedFlags = new[]
            {
                "allowOverride"
            };

            base.OnResolve(fileOrigin);
            propertyValue = GetParameter("value");
        }
    }
}
