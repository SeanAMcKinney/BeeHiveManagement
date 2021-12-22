using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeHiveManagement
{

    static class honeyVault
    {
        public const float NECTAR_CONVERSION_RATIO = .19f;
        public const float LOW_LEVEL_WARNING = 10f;
        private static float honey = 25f;
        private static float nectar = 100f;

        public static void CollectNectar(float amount)
        {
            if (amount > 0f) nectar += amount;
        }

        public static void ConvertNectarToHoney(float amount)
        {
            float nectarToConvert = amount;
            if (nectarToConvert < 0f) nectarToConvert = nectar;
            nectar -= nectarToConvert;
            honey += nectarToConvert * NECTAR_CONVERSION_RATIO;
        }

        public static bool ConsumeHoney(float amount)
        {
            if (honey > amount)
            {
                honey -= amount;
                return true;
            }
            return false;
        }

        public static string StatusReport
        {
            get
            {
                string status = $"{honey:0.0} units of honey\n" + $"{nectar:0.0} units of nectar\n";
                string warnings = "";
                if (honey < LOW_LEVEL_WARNING) warnings += "\nLOW HONEY - ADD A HONEY MANUFACTURER";
                if (nectar < LOW_LEVEL_WARNING) warnings += "\nLOW NECTAR - ADD A NECTAR COLLECTOR";
                return status + warnings;
            }
        }

    class Bee
    {

    }

    class Queen : Bee
    {

    }

    class NectarCollector : Bee
    {

    }

    class HoneyManufacturer : Bee
    {

    }

    class EggCare : Bee
    {

    }
}
