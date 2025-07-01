using BackendGeems.Application;
using BackendGeems.Domain;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace UnitTests
{
    public class EliminarEmpresaTest
    {
        private Mock<IEmpresaRepo> _empresaRepoMock;
        private Mock<IPagoRepo> _pagoRepoMock;
        private Mock<IBeneficioRepo> _beneficioRepoMock;
        private Mock<IEmpleadoRepo> _empleadoRepoMock;
        private Mock<IReporteService> _reporteServiceMock;
        private QueryEmpresa _queryEmpresa;

        [SetUp]
        public void Setup()
        {
            _empresaRepoMock = new Mock<IEmpresaRepo>();
            _pagoRepoMock = new Mock<IPagoRepo>();
            _beneficioRepoMock = new Mock<IBeneficioRepo>();
            _empleadoRepoMock = new Mock<IEmpleadoRepo>();
            _reporteServiceMock = new Mock<IReporteService>();

            _queryEmpresa = new QueryEmpresa(
                _empresaRepoMock.Object,
                _pagoRepoMock.Object,
                _beneficioRepoMock.Object,
                _empleadoRepoMock.Object,
                _reporteServiceMock.Object
            );
        }

        [Test]
        public void EliminarEmpresa_SinPagos_RealizaBorradoLogico()
        {
            var cedula = "123";
            _empresaRepoMock.Setup(r => r.GetEmpleados(cedula)).Returns(new List<Empleado>());
            _empresaRepoMock.Setup(r => r.BorradoLogico(cedula));

            _queryEmpresa.EliminarEmpresa(cedula);

            _empresaRepoMock.Verify(r => r.BorradoLogico(cedula), Times.Once);
            _empresaRepoMock.Verify(r => r.BorradoFisico(cedula), Times.Never);
        }

        [Test]
        public void EliminarEmpresa_ConPagos_RealizaBorradoFisico()
        {
            var cedula = "123";
            var empleados = new List<Empleado>
            {
                new Empleado { Id = Guid.NewGuid(), CedulaPersona = 1 }
            };
            _empresaRepoMock.Setup(r => r.GetEmpleados(cedula)).Returns(empleados);
            _pagoRepoMock.Setup(r => r.ContarPagos(It.IsAny<Guid>())).Returns(1);
            _empresaRepoMock.Setup(r => r.BorradoFisico(cedula));

            _queryEmpresa.EliminarEmpresa(cedula);

            _empresaRepoMock.Verify(r => r.BorradoFisico(cedula), Times.Once);
            _empresaRepoMock.Verify(r => r.BorradoLogico(cedula), Times.Never);
        }

        [Test]
        public void EliminarEmpresa_ThrowsException_PropagaExcepcion()
        {
            var cedula = "123";
            _empresaRepoMock.Setup(r => r.GetEmpleados(cedula)).Throws(new Exception("Error"));

            Assert.Throws<Exception>(() => _queryEmpresa.EliminarEmpresa(cedula));
        }
    }
}
