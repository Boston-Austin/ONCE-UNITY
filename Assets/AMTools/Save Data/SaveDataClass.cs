using System;
using System.Collections.Generic;

namespace AMTools.AMToolsSaveManager
{
    [Serializable]
    public class SaveDataClass
    {
        [Serializable]
        public class SaveDataInt
        {
            public string _key = "Key";
            public int _int = 0;
        }

        [Serializable]
        public class SaveDataFloat
        {
            public string _key = "Key";
            public float _float = 0;
        }

        [Serializable]
        public class SaveDataBool
        {
            public string _key = "Key";
            public bool _bool = false;
        }

        [Serializable]
        public class SaveDataString
        {
            public string _key = "Key";
            public string _string = "";
        }

        public List<SaveDataInt> _saveDataIntList;
        public List<SaveDataFloat> _saveDataFloatList;
        public List<SaveDataBool> _saveDataBoolList;
        public List<SaveDataString> _saveDataStringList;
    }
}