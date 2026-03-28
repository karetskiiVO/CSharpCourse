namespace CarInformatorApp {
    public interface ICar {
        string brand { get; }
        string GetDescription();
    }

    public interface IElectric {
        float batteryKWh { get; }
        float power { get; }
    }

    public interface IGas {
        public enum FuelType {
            Petrol,
            Diesel,
            Hybrid,
            Other,
        }

        FuelType fuelType { get; }
        float power { get; }
    }

    public interface IMechanicalTransmission {
        int gears { get; }
    }

    public interface IAutomaticalTransmission {
        public enum Mode {
            Automatic,
            AMT,
            CVT,
            Electric,
            Other,
        }

        Mode transmissionMode { get; }
        string? additionalTransmissionInfo => null;
    }

    public abstract class ACar : ICar {
        protected string? brand;
        protected string? model;
        protected int seats;

        string ICar.brand => brand ?? "Unknown brand";

        public virtual string GetDescription() {
            var descriptionBuilder = new System.Text.StringBuilder();

            descriptionBuilder
                .AppendLine($"{brand ?? "<Unknown brand>"} {model ?? "<Unknown model>"}:")
                .AppendLine($"\tКоличество мест: {seats}");

            var electric = this as IElectric;
            var gas = this as IGas;

            if (electric != null && gas != null) {
                descriptionBuilder
                    .AppendLine("\tГибрид:")
                    .AppendLine($"\t\tТип топлива: {gas.fuelType}")
                    .AppendLine($"\t\tМощность ДВС: {gas.power} л.с.")
                    .AppendLine($"\t\tЕмкость батареи: {electric.batteryKWh} кВт*ч");
            } else if (electric != null) {
                descriptionBuilder
                    .AppendLine($"\tЭлектромобиль:")
                    .AppendLine($"\t\tМощность: {electric.power} л.с.")
                    .AppendLine($"\t\tЕмкость батареи: {electric.batteryKWh} кВт*ч");
            } else if (gas != null) {
                descriptionBuilder
                    .AppendLine($"\tДВС:")
                    .AppendLine($"\t\tТип топлива: {gas.fuelType}")
                    .AppendLine($"\t\tМощность: {gas.power} л.с.");
            } else {
                descriptionBuilder.AppendLine("\tВелосипед");
            }

            if (this is IAutomaticalTransmission automatic) {
                descriptionBuilder
                    .AppendLine($"\tАКПП: {automatic.transmissionMode}");

                if (!string.IsNullOrEmpty(automatic.additionalTransmissionInfo)) {
                    descriptionBuilder.AppendLine($"\t\t{automatic.additionalTransmissionInfo}");
                }
            }

            if (this is IMechanicalTransmission mechanical) {
                descriptionBuilder
                    .AppendLine($"\tМКПП: {mechanical.gears} передач");
            }

            return descriptionBuilder.ToString();
        }
    }

    public abstract class GasCar : ACar, IGas {
        public abstract IGas.FuelType fuelType { get; }
        public abstract float power { get; }
    }

    public abstract class ElectricCar : ACar, IElectric, IAutomaticalTransmission {
        public abstract float batteryKWh { get; }
        public abstract float power { get; }
        public virtual IAutomaticalTransmission.Mode transmissionMode => IAutomaticalTransmission.Mode.Electric;
    }

    public abstract class HybridCar : ACar, IGas, IElectric, IAutomaticalTransmission {
        public abstract IGas.FuelType fuelType { get; }
        public abstract float power { get; }
        public abstract float batteryKWh { get; }
        public virtual IAutomaticalTransmission.Mode transmissionMode => IAutomaticalTransmission.Mode.Electric;
    }

    public class Tesla : ElectricCar {
        public Tesla() {
            brand = "Tesla";
            model = "Model S";
            seats = 5;
        }

        public override float batteryKWh => 100;
        public override float power => 150;
    }

    public class Honda : HybridCar {
        public Honda() {
            brand = "Honda";
            model = "Civic type R";
            seats = 5;
        }

        public override IGas.FuelType fuelType => IGas.FuelType.Petrol;
        public override float power => 120;

        public override float batteryKWh => 15;
    }

    public class Lada : GasCar, IMechanicalTransmission {
        public Lada() {
            brand = "Lada";
            model = "Niva";
            seats = 4;
        }

        public override IGas.FuelType fuelType => IGas.FuelType.Petrol;
        public override float power => 80;

        public int gears => 5;
    }

    public class Wolksvagen : GasCar, IAutomaticalTransmission {
        public Wolksvagen() {
            brand = "Wolksvagen";
            model = "Golf";
            seats = 5;
        }

        public override IGas.FuelType fuelType => IGas.FuelType.Petrol;
        public override float power => 115;

        public IAutomaticalTransmission.Mode transmissionMode => IAutomaticalTransmission.Mode.AMT;
    }

    public class Toyota : GasCar, IAutomaticalTransmission {
        public Toyota() {
            brand = "Toyota";
            model = "Corola";
            seats = 7;
        }

        public override IGas.FuelType fuelType => IGas.FuelType.Petrol;
        public override float power => 110;

        public IAutomaticalTransmission.Mode transmissionMode => IAutomaticalTransmission.Mode.CVT;
    }

    public static class CarFactory {
        public enum CarType {
            Tesla,
            Honda,
            Toyota,
            Wolksvagen,
            Lada,
        }

        public static ICar CreateCar(CarType carType) {
            return carType switch {
                CarType.Tesla => new Tesla(),
                CarType.Honda => new Honda(),
                CarType.Lada => new Lada(),
                CarType.Wolksvagen => new Wolksvagen(),
                CarType.Toyota => new Toyota(),
                _ => throw new ArgumentException("Unknown car type"),
            };
        }
    }
}
