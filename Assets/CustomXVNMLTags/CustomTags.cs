using XVNML.Core.Tags;
using XVNML.XVNMLUtility.Tags;

namespace XVNML2U
{
    [AssociateWithTag("struct", typeof(Proxy), TagOccurance.Multiple, true)]
    public class StructTag : UserDefined
    {
        internal string structName => tagName;
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
    public class PropertyTag : UserDefined
    {
        internal string propertyName => tagName;
        internal object propertyValue;

        public override void OnResolve(string fileOrigin)
        {
            base.OnResolve(fileOrigin);
            propertyValue = parameterInfo["value"];
        }
    }
}
