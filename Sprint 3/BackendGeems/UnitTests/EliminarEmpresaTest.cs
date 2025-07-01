using BackendGeems.Application;
using BackendGeems.Domain;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var cedula = "EMP123";
            var empleados = new List<Empleado>
            {
                new Empleado { CedulaPersona = 1, Id = Guid.NewGuid() }
            };

            _empresaRepoMock.Setup(r => r.GetEmpleados(cedula)).Returns(empleados);
            _pagoRepoMock.Setup(r => r.ContarPagos(It.IsAny<Guid>())).Returns(0);
            _beneficioRepoMock.Setup(r => r.GetCompanyBenefits(cedula)).Returns(new List<object>());
            _beneficioRepoMock.Setup(r => r.EliminarBeneficio(It.IsAny<string>()));
            _empleadoRepoMock.Setup(r => r.BorrarLogicoEmpleado(It.IsAny<string>()));
            _empleadoRepoMock.Setup(r => r.BorrarPermanenteEmpleado(It.IsAny<string>()));
            _empleadoRepoMock.Setup(r => r.UsuarioEstaBorrado(It.IsAny<string>())).Returns(false);
            _empleadoRepoMock.Setup(r => r.ObtenerNombreEmpleadoPorCedula(It.IsAny<string>())).Returns("Empleado Test");
            _empleadoRepoMock.Setup(r => r.ObtenerCorreoEmpleadoPorCedula(It.IsAny<string>())).Returns("empleado@test.com");
            _empleadoRepoMock.Setup(r => r.ObtenerNombreEmpresaPorCedula(It.IsAny<string>())).Returns("Empresa Test");
            _empleadoRepoMock.Setup(r => r.VerificarRelacionEmpleadoPlanilla(It.IsAny<string>())).Returns(false);
            _empleadoRepoMock.Setup(r => r.BorrarTimesheetEmpleado(It.IsAny<string>(), It.IsAny<bool>()));
            _reporteServiceMock.Setup(r => r.EnviarCorreoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            _empresaRepoMock.Setup(r => r.BorradoLogico(cedula));

            _queryEmpresa.EliminarEmpresa(cedula);

            _empresaRepoMock.Verify(r => r.BorradoLogico(cedula), Times.Once);
            _empresaRepoMock.Verify(r => r.BorradoFisico(cedula), Times.Never);
        }

        [Test]
        public void EliminarEmpresa_ConPagos_RealizaBorradoFisico()
        {
            var cedula = "EMP456";
            var empleados = new List<Empleado>
            {
                new Empleado { CedulaPersona = 2, Id = Guid.NewGuid() }
            };

            _empresaRepoMock.Setup(r => r.GetEmpleados(cedula)).Returns(empleados);
            _pagoRepoMock.Setup(r => r.ContarPagos(It.IsAny<Guid>())).Returns(1);
            _beneficioRepoMock.Setup(r => r.GetCompanyBenefits(cedula)).Returns(new List<object>());
            _beneficioRepoMock.Setup(r => r.EliminarBeneficio(It.IsAny<string>()));
            _empleadoRepoMock.Setup(r => r.BorrarLogicoEmpleado(It.IsAny<string>()));
            _empleadoRepoMock.Setup(r => r.BorrarPermanenteEmpleado(It.IsAny<string>()));
            _empleadoRepoMock.Setup(r => r.UsuarioEstaBorrado(It.IsAny<string>())).Returns(false);
            _empleadoRepoMock.Setup(r => r.ObtenerNombreEmpleadoPorCedula(It.IsAny<string>())).Returns("Empleado Test");
            _empleadoRepoMock.Setup(r => r.ObtenerCorreoEmpleadoPorCedula(It.IsAny<string>())).Returns("empleado@test.com");
            _empleadoRepoMock.Setup(r => r.ObtenerNombreEmpresaPorCedula(It.IsAny<string>())).Returns("Empresa Test");
            _empleadoRepoMock.Setup(r => r.VerificarRelacionEmpleadoPlanilla(It.IsAny<string>())).Returns(true);
            _empleadoRepoMock.Setup(r => r.BorrarTimesheetEmpleado(It.IsAny<string>(), It.IsAny<bool>()));
            _reporteServiceMock.Setup(r => r.EnviarCorreoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            _empresaRepoMock.Setup(r => r.BorradoFisico(cedula));

            _queryEmpresa.EliminarEmpresa(cedula);

            _empresaRepoMock.Verify(r => r.BorradoFisico(cedula), Times.Once);
            _empresaRepoMock.Verify(r => r.BorradoLogico(cedula), Times.Never);
        }

        [Test]
        public void EliminarEmpresa_ThrowsException_PropagaExcepcion()
        {
            var cedula = "EMP789";
            _empresaRepoMock.Setup(r => r.GetEmpleados(cedula)).Throws(new Exception("Error"));

            Assert.Throws<Exception>(() => _queryEmpresa.EliminarEmpresa(cedula));
        }
    }
}
