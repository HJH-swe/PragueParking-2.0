using PragueParking.Core;

namespace PragueParking.Tests
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void IsEnoughSpace_AddCarToEmptySpace_ReturnTrue()
        {
            // Arrange 
            // Create car and parking space with required parameters
            Car myCar = new Car("ABC123", 4, 20);
            ParkingSpace myParkingSpace = new ParkingSpace(4, 20, new List<Vehicle>());

            // Act
            // Park car in space
            var result = myParkingSpace.IsEnoughSpace(myCar);

            // Assert
            // Expected result: true
            Assert.IsTrue(result);

        }
        [TestMethod]
        public void AddVehicle_AddCarToEmptySpace_AvailableSpaceResultZero()
        {
            // Arrange 
            // Create car and parking space with required parameters
            Car myCar = new Car("ABC123", 4, 20);
            ParkingSpace myParkingSpace = new ParkingSpace(4, 20, new List<Vehicle>());

            // Act
            // Add myCar to parking space, check available size
            myParkingSpace.AddVehicle(myCar);
            int result = myParkingSpace.AvailableSize;
            int expected = 0;

            // Assert
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void AddVehicle_AddCarToOccupiedSpace_ResultFalse()
        {
            // Arrange 
            // Create 2 cars and a parking space, and add first car
            Car car1 = new Car("ABC123", 4, 20);
            Car car2 = new Car("XYZ789", 4, 20);
            ParkingSpace myParkingSpace = new ParkingSpace(4, 20, new List<Vehicle>());
            myParkingSpace.AddVehicle(car1);

            // Act
            // Try to add car2 to myParkingSpace
            var result = myParkingSpace.AddVehicle(car2);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
