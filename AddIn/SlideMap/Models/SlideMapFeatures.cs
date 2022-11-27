using System.Collections.Generic;

namespace SlideMap.Models
{
    abstract class SlideMapFeatures
    {
        protected Dictionary<string, object> _attributes;

        protected SlideMapFeatures()
        {
            _attributes = new Dictionary<string, object>();
        }

        public abstract Dictionary<string, object> GetAttributes();
    }
}
