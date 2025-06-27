using NUnit.Framework;
using Moq;
using BackendGeems.Application;
using BackendGeems.Infraestructure;
using System;

namespace BackendGeems.Tests
{
    public class BorradoDeEmpleadosTests
    {
        private Mock<IEmpleadoRepo> _mockRepo;
        private BorradoDeEmpleados _servicio;
        private string _cedula;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IEmpleadoRepo>();
            _servicio = new BorradoDeEmpleados(_mockRepo.Object);
            _cedula = "123456789";
        }

        [Test]
        public void BorrarEmpleado_ConPagos_DeberiaHacerBorradoLogico()
        {
           
            _mockRepo.Setup(r => r.VerificarRelacionEmpleadoPlanilla(_cedula)).Returns(true);

            
            var resultado = _servicio.BorrarEmpleado(_cedula);

            
            Assert.AreEqual("Borrado logico completado", resultado);
            _mockRepo.Verify(r => r.BorrarLogicoEmpleado(_cedula), Times.Once);
            _mockRepo.Verify(r => r.BorrarPermanenteEmpleado(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void BorrarEmpleado_SinPagos_DeberiaHacerBorradoPermanente()
        {
         
            _mockRepo.Setup(r => r.VerificarRelacionEmpleadoPlanilla(_cedula)).Returns(false);

           
            var resultado = _servicio.BorrarEmpleado(_cedula);

           
            Assert.AreEqual("Borrado Permanente completado", resultado);
            _mockRepo.Verify(r => r.BorrarPermanenteEmpleado(_cedula), Times.Once);
            _mockRepo.Verify(r => r.BorrarLogicoEmpleado(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void BorrarEmpleado_CuandoRepoFalla_LanzaArgumentException()
        {
        
            _mockRepo.Setup(r => r.VerificarRelacionEmpleadoPlanilla(_cedula))
                     .Throws(new Exception("Error inesperado"));

            
            var ex = Assert.Throws<ArgumentException>(() => _servicio.BorrarEmpleado(_cedula));
            Assert.AreEqual("Error inesperado", ex.Message);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public void UsuarioActivo_DeberiaDevolverResultadoCorrecto(bool estaBorrado, bool esperado)
        {
            
            _mockRepo.Setup(r => r.UsuarioEstaBorrado(_cedula)).Returns(estaBorrado);

           
            var resultado = _servicio.UsuarioActivo(_cedula);

            
            Assert.AreEqual(esperado, resultado);
        }
    }
}
