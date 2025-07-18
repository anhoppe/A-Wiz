﻿using Awiz.Core.ClassDiagram;
using Awiz.Core.Contract.CodeInfo;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;

namespace Awiz.Core.Test.ClassDiagram
{
    [TestFixture]
    public class ClassNodeGeneratorTest
    {
        private Mock<IGraph> _graphMock = new();

        private ClassNodeGenerator _sut = new ClassNodeGenerator();

        [SetUp]
        public void SetUp()
        { 
            _graphMock = new();

            _sut = new ClassNodeGenerator();
        }

        [Test]
        public void CreateAssociation_WhenAssociationIsAddedForExistingClasses_ThenAssiciationIsAddedToTheGraph()
        {
            // Arrange
            var node1Mock = new Mock<INode>();
            var node2Mock = new Mock<INode>();

            var classInfo1 = new ClassInfo()
            {
                Name = "foo",
            };

            var classInfo2 = new ClassInfo()
            {
                Name = "bar"
            };

            _sut.NodeToClassInfoMapping = new Dictionary<INode, ClassInfo>()
            {
                [node1Mock.Object] = classInfo1,
                [node2Mock.Object] = classInfo2,
            };

            var edgeBuilderMock = new Mock<IEdgeBuilder>();

            _graphMock.Setup(m => m.AddEdge(node1Mock.Object, node2Mock.Object))
                .Returns(edgeBuilderMock.Object);

            // Act
            _sut.CreateAssociation(_graphMock.Object, classInfo1, classInfo2);

            // Assert
            edgeBuilderMock.Verify(m => m.Build(), "EdgeBuilder.Build should have been called because it finalizes the creation of the association");
        }

        [Test]
        public void CreateAssociationWithMultiplicity_WhenAssociationWithMultiplicityIsAdded_ThenEdgeWithFromToLabelsAreAdded()
        {
            // Arrange
            var node1Mock = new Mock<INode>();
            var node2Mock = new Mock<INode>();

            var classInfo1 = new ClassInfo()
            {
                Name = "foo",
            };

            var classInfo2 = new ClassInfo()
            {
                Name = "bar"
            };

            _sut.NodeToClassInfoMapping = new Dictionary<INode, ClassInfo>()
            {
                [node1Mock.Object] = classInfo1,
                [node2Mock.Object] = classInfo2,
            };

            var edgeBuilderMock = new Mock<IEdgeBuilder>();
            edgeBuilderMock.Setup(m => m.WithFromLabel("foo")).Returns(edgeBuilderMock.Object);
            edgeBuilderMock.Setup(m => m.WithToLabel("bar")).Returns(edgeBuilderMock.Object);
            edgeBuilderMock.Setup(m => m.WithLabelOffsetPerCent(It.IsAny<float>())).Returns(edgeBuilderMock.Object);

            _graphMock.Setup(m => m.AddEdge(node1Mock.Object, node2Mock.Object)).Returns(edgeBuilderMock.Object);

            // Act
            _sut.CreateAssociation(_graphMock.Object, classInfo1, classInfo2, "foo", "bar");

            // Assert
            edgeBuilderMock.Verify(m => m.Build(), "Build should have been called because the edgeBuilder is setup to return this when the expected methods with parameters were called, which finally results in the Build call");
        }

        [Test]
        public void CreateImplementation_WhenImplementationIsAddedForClasses_ThenAnEdgeWithClosedArrowAndDashedLineInDirectionOfInterfaceIsInserted()
        {
            // Arrange
            var node1Mock = new Mock<INode>();
            var node2Mock = new Mock<INode>();

            var classInfo1 = new ClassInfo()
            {
                Name = "foo",
                Type = ClassType.Interface,
            };

            var classInfo2 = new ClassInfo()
            {
                Name = "bar",
                Type = ClassType.Class,
            };

            _sut.NodeToClassInfoMapping = new Dictionary<INode, ClassInfo>()
            {
                [node1Mock.Object] = classInfo1,
                [node2Mock.Object] = classInfo2,
            };

            var edgeBuilderMock = new Mock<IEdgeBuilder>();
            edgeBuilderMock.Setup(m => m.WithEnding(Ending.ClosedArrow)).Returns(edgeBuilderMock.Object);
            edgeBuilderMock.Setup(m => m.WithStyle(Style.Dashed)).Returns(edgeBuilderMock.Object);

            _graphMock.Setup(m => m.AddEdge(node2Mock.Object, node1Mock.Object)).Returns(edgeBuilderMock.Object);

            // Act
            _sut.CreateImplementation(_graphMock.Object, classInfo1, classInfo2);

            // Assert
            edgeBuilderMock.Verify(m => m.Build(), "Build should have been called because the edgeBuilder is setup to return this when the expected methods with parameters were called, which finally results in the Build call");
        }

        [Test]
        public void CreateExtension_WhenExtensionIsAddedForClasses_ThenAnEdgeWithClosedArrowIsInserted()
        {
            // Arrange
            var node1Mock = new Mock<INode>();
            var node2Mock = new Mock<INode>();

            var classInfo1 = new ClassInfo()
            {
                Name = "foo",
                Type = ClassType.Class,
            };

            var classInfo2 = new ClassInfo()
            {
                Name = "bar",
                Type = ClassType.Class,
            };

            _sut.NodeToClassInfoMapping = new Dictionary<INode, ClassInfo>()
            {
                [node1Mock.Object] = classInfo1,
                [node2Mock.Object] = classInfo2,
            };

            var edgeBuilderMock = new Mock<IEdgeBuilder>();
            edgeBuilderMock.Setup(m => m.WithEnding(Ending.ClosedArrow)).Returns(edgeBuilderMock.Object);

            _graphMock.Setup(m => m.AddEdge(node2Mock.Object, node1Mock.Object)).Returns(edgeBuilderMock.Object);

            // Act
            _sut.CreateExtension(_graphMock.Object, classInfo1, classInfo2);

            // Assert
            edgeBuilderMock.Verify(m => m.Build(), "Build should have been called because the edgeBuilder is setup to return this when the expected methods with parameters were called, which finally results in the Build call");

        }

        [Test]
        public void UpdateClassNode_WhenCalled_ThenNodeContentSetToClassInfo()
        {
            // Arrange
            var nodeMock = new Mock<INode>();

            var gridMock = new Mock<IGrid>();
            var cell1 = new Mock<IGridCell>();
            var cell2 = new Mock<IGridCell>();
            var cell3 = new Mock<IGridCell>();

            gridMock.Setup(m => m.Cells).Returns(new IGridCell[1, 3]
            {
                { 
                    cell1.Object, 
                    cell2.Object, 
                    cell3.Object 
                }
            });

            nodeMock.Setup(p => p.Grid).Returns(gridMock.Object);

            var buttonMock = new Mock<IButton>();
            nodeMock.Setup(p => p.GetButtonById("VersionUpdateInfo")).Returns(buttonMock.Object);

            var classInfo = new ClassInfo()
            {
                Name = "foo",
                Properties =
                [
                    new PropertyInfo()
                    {
                        AccessModifier = "public",
                        Name = "bar",
                        TypeName = "barbar",
                    }
                ],
                Methods =
                [
                    new MethodInfo()
                    {
                        AccessModifier = "public",
                        Name = "buz",
                        ReturnType = "buzbuz",
                    }
                ]
            };

            // Act
            _sut.UpdateClassNode(nodeMock.Object, classInfo, (_) => { });

            // Assert
            cell1.VerifySet(p => p.Text = "foo");
            cell2.VerifySet(p => p.Text = "+ barbar bar\n");
            cell3.VerifySet(p => p.Text = "+ buz(): buzbuz\n");
        }

        [Test]
        public void UpdateClassNode_WhenClassInfoHasVerisonUpdates_ThenTheNodeButtonIsEnabledAndChangeEventIsRegistered()
        {
            // Arrange
            var nodeMock = new Mock<INode>();

            var gridMock = new Mock<IGrid>();
            var cell1 = new Mock<IGridCell>();
            var cell2 = new Mock<IGridCell>();
            var cell3 = new Mock<IGridCell>();

            gridMock.Setup(m => m.Cells).Returns(new IGridCell[1, 3]
            {
                {
                    cell1.Object,
                    cell2.Object,
                    cell3.Object
                }
            });

            nodeMock.Setup(p => p.Grid).Returns(gridMock.Object);

            var buttonMock = new Mock<IButton>();
            nodeMock.Setup(p => p.GetButtonById("VersionUpdateInfo")).Returns(buttonMock.Object);

            var classInfo = new ClassInfo()
            {
                Name = "foo",
                AddedProperties =
                [
                    new PropertyInfo()
                    {
                        AccessModifier = "public",
                        Name = "bar",
                        TypeName = "barbar",
                    }
                ],
            };

            bool callbackCalled = false;

            // Act
            _sut.UpdateClassNode(nodeMock.Object, classInfo, (ci) =>
            {
                if (ci.Name == "foo")
                {
                    callbackCalled = true;
                }
            });

            buttonMock.Raise(p => p.Clicked += null, EventArgs.Empty);

            // Assert
            Assert.That(callbackCalled);
            buttonMock.VerifySet(p => p.Visible = true);
        }


        [Test]
        public void UpdateClassNode_WhenClassInfoHasNoVerisonUpdates_ThenVersionUpdateInfoButtonIsInvisible()
        {
            // Arrange
            var nodeMock = new Mock<INode>();

            var gridMock = new Mock<IGrid>();
            var cell1 = new Mock<IGridCell>();
            var cell2 = new Mock<IGridCell>();
            var cell3 = new Mock<IGridCell>();

            gridMock.Setup(m => m.Cells).Returns(new IGridCell[1, 3]
            {
                {
                    cell1.Object,
                    cell2.Object,
                    cell3.Object
                }
            });

            nodeMock.Setup(p => p.Grid).Returns(gridMock.Object);

            var buttonMock = new Mock<IButton>();
            nodeMock.Setup(p => p.GetButtonById("VersionUpdateInfo")).Returns(buttonMock.Object);

            var classInfo = new ClassInfo()
            {
                Name = "foo",
            };

            // Act
            _sut.UpdateClassNode(nodeMock.Object, classInfo, (_) => {});

            // Assert
            buttonMock.VerifySet(p => p.Visible = false);
        }
    }
}
