using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeHiveManagement
{

    static class HoneyVault
    {
        public const float NECTAR_CONVERSION_RATIO = .75f;
        public const float LOW_LEVEL_WARNING = 10f;
        private static float honey = 100f;
        private static float nectar = 50f;

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
    }

        class Bee
        {
            public virtual float CostPerShift { get; }

            public string Job { get; private set; }

            public Bee(string job)
            {
                Job = job;
            }

            public void WorkTheNextShift()
            {
                if (HoneyVault.ConsumeHoney(CostPerShift))
                {
                    DoJob();
                }
            }

            protected virtual void DoJob() { /* the subclass overrides this */ }
        }
    
        class Queen : Bee
        {
            public const float EGGS_PER_SHIFT = 0.45f;
            public const float HONEY_PER_UNASSIGNED_WORKER = 0.5f;

            private Bee[] workers = new Bee[0];
            private float eggs = 0;
            private float unassignedWorkers = 4;

            public string StatusReport { get; private set; }
            public override float CostPerShift { get { return 2.15f;  } }

            public Queen() : base("Queen")
            {
                AssignBee("Nectar Collector");
                AssignBee("Honey Manufacturer");
                AssignBee("Egg Care");
            }

            private void AddWorker(Bee worker)
        ///<summary>
        ///Expand the workers array by one slot and add a Bee reference.
        ///<summary>
        ///<param name="worker">Worker to add to the workers array.</param>
            {
                if (unassignedWorkers >= 1)
                {
                    unassignedWorkers--;
                    Array.Resize(ref workers, workers.Length + 1);
                    workers[workers.Length - 1] = worker;
                }
                
            }

            private void UpdateStatusReport()
            {
                StatusReport = $"Vault report: \n{HoneyVault.StatusReport}\n" +
                $"\nEgg Count: {eggs:0.0}\nUnassigned workers: {unassignedWorkers:0.0}\n" +
                $"{WorkerStatus("Nectar Collector")}\n{WorkerStatus("Honey Manufacturer")}" +
                $"\n{WorkerStatus("Egg Care")}\nTOTAL WORKERS: {workers.Length}";
            }

            public void CareForEggs(float eggsToConvert)
            {
                if (eggs >= eggsToConvert)
                {
                    eggs -= eggsToConvert;
                    unassignedWorkers += eggsToConvert;
                }
            }

            private string WorkerStatus(string job)
            {
                int count = 0;
                foreach (Bee worker in workers)         
                    if (worker.Job == job) count++;
                string s = "s";
                if (count == 1) s = "";
                return $"{count} {job} bee{s}";
            }

            public void AssignBee(string job)
            {
                switch (job)
                {
                    case "Nectar Collector":
                        AddWorker(new NectarCollector());
                        break;
                    case "Honey Manufacturer":
                        AddWorker(new HoneyManufacturer());
                        break;
                    case "Egg Care":
                        AddWorker(new EggCare(this));
                        break;
                }
                UpdateStatusReport();
            }

            protected override void DoJob()
            {
                eggs += EGGS_PER_SHIFT;
                foreach (Bee worker in workers)
                {
                    worker.WorkTheNextShift();
                }
                HoneyVault.ConsumeHoney(unassignedWorkers * HONEY_PER_UNASSIGNED_WORKER);
                UpdateStatusReport();
            }
        }



        class NectarCollector : Bee
        {
            public const float NECTAR_COLLECTED_PER_SHIFT = 33.25f;
            public override float CostPerShift { get { return 1.95f; } }
            public NectarCollector() : base("Nectar Collector") { }

            protected override void DoJob()
            {
                HoneyVault.CollectNectar(NECTAR_COLLECTED_PER_SHIFT);
            }
        }

        class HoneyManufacturer : Bee
        {
            public const float NECTAR_COLLECTED_PER_SHIFT = 33.15F;
            public override float CostPerShift { get { return 1.7f; } }
            public HoneyManufacturer() : base("Honey Manufacturer") { }

            protected override void DoJob()
            {
                HoneyVault.ConvertNectarToHoney(CostPerShift);
            }
        }

        class EggCare : Bee
        {
            public const float CARE_PROGRESS_PER_SHIFT = 0.15f;
            public override float CostPerShift { get { return 0.15f; } }

            private Queen queen;

            public EggCare(Queen queen) : base("Egg Care")
            {
                this.queen = queen;
            }

            protected override void DoJob()
            {
                queen.CareForEggs(CARE_PROGRESS_PER_SHIFT);
            }
        }
    
}