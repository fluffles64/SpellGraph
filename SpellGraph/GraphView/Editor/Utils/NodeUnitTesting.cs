using NUnit.Framework;
using System.Collections.Generic;

/// <summary>
/// This utility class utilizes the NUnit framework to unit test custom-made nodes. You can open
/// the test runner in Unity by going to Window > General > Test Runner.
/// By going into EditMode and pressing "Run All", you can test whether the nodes return what you expect.
/// </summary>

[TestFixture]
public class NodeUnitTesting
{
    #region Math

        #region Basic

            #region Add
            [Test]
            public void MathBasicAddNode_AddTwoFloats()
            {
                // Arrange
                var addNode = new RuntimeMathBasicAddNode();
                var portValues = new List<object> { 5f, 3f };

                // Act
                var result = addNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(8f, result);
            }

            [Test]
            public void MathBasicAddNode_AddIntAndFloat()
            {
                // Arrange
                var addNode = new RuntimeMathBasicAddNode();
                var portValues = new List<object> { 5, 3.5f };

                // Act
                var result = addNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(8.5f, result);
            }

            [Test]
            public void MathBasicAddNode_HandleOneNullValue()
            {
                // Arrange
                var addNode = new RuntimeMathBasicAddNode();
                var portValues = new List<object> { null, 3f };

                // Act
                var result = addNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(3f, result);
            }

            [Test]
            public void MathBasicAddNode_HandleAllNullValues()
            {
                // Arrange
                var addNode = new RuntimeMathBasicAddNode();
                var portValues = new List<object> { null, null };

                // Act
                var result = addNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(0f, result);
            }
    #endregion

            #region Subtract
            [Test]
            public void MathBasicSubtractNode_SubtractTwoFloats()
            {
                // Arrange
                var subtractNode = new RuntimeMathBasicSubtractNode();
                var portValues = new List<object> { 5f, 3f };

                // Act
                var result = subtractNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(2f, result);
            }

            [Test]
            public void MathBasicSubtractNode_SubtractIntAndFloat()
            {
                // Arrange
                var subtractNode = new RuntimeMathBasicSubtractNode();
                var portValues = new List<object> { 5, 3.5f };

                // Act
                var result = subtractNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(1.5f, result);
            }

            [Test]
            public void MathBasicSubtractNode_HandleOneNullValue()
            {
                // Arrange
                var subtractNode = new RuntimeMathBasicSubtractNode();
                var portValues = new List<object> { null, 3f };

                // Act
                var result = subtractNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(-3f, result);
            }

            [Test]
            public void MathBasicSubtractNode_HandleAllNullValues()
            {
                // Arrange
                var subtractNode = new RuntimeMathBasicSubtractNode();
                var portValues = new List<object> { null, null };

                // Act
                var result = subtractNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(0f, result);
            }
            #endregion

            #region Divide
            [Test]
            public void MathBasicDivideNode_DivideTwoFloats()
            {
                // Arrange
                var divideNode = new RuntimeMathBasicDivideNode();
                var portValues = new List<object> { 10f, 2f };

                // Act
                var result = divideNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(5f, result);
            }

            [Test]
            public void MathBasicDivideNode_DivideIntAndFloat()
            {
                // Arrange
                var divideNode = new RuntimeMathBasicDivideNode();
                var portValues = new List<object> { 10, 2.5f };

                // Act
                var result = divideNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(4f, result);
            }

            [Test]
            public void MathBasicDivideNode_HandleDividedByZero()
            {
                // Arrange
                var divideNode = new RuntimeMathBasicDivideNode();
                var portValues = new List<object> { 50f, 0f };

                // Act
                var result = divideNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(float.PositiveInfinity, result);
            }

            [Test]
            public void MathBasicDivideNode_HandleNullDividedByZero()
            {
                // Arrange
                var divideNode = new RuntimeMathBasicDivideNode();
                var portValues = new List<object> { null, 0f };

                // Act
                var result = divideNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(float.NaN, result);
            }

            [Test]
            public void MathBasicDivideNode_HandleOneNullValue()
            {
                // Arrange
                var divideNode = new RuntimeMathBasicDivideNode();
                var portValues = new List<object> { null, 2f };

                // Act
                var result = divideNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(0f, result);
            }

            [Test]
            public void MathBasicDivideNode_HandleAllNullValues()
            {
                // Arrange
                var divideNode = new RuntimeMathBasicDivideNode();
                var portValues = new List<object> { null, null };

                // Act
                var result = divideNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(0f, result);
            }
            #endregion

            #region Multiply
            [Test]
            public void MathBasicMultiplyNode_MultiplyTwoFloats()
            {
                // Arrange
                var multiplyNode = new RuntimeMathBasicMultiplyNode();
                var portValues = new List<object> { 5f, 3f };

                // Act
                var result = multiplyNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(15f, result);
            }

            [Test]
            public void MathBasicMultiplyNode_MultiplyIntAndFloat()
            {
                // Arrange
                var multiplyNode = new RuntimeMathBasicMultiplyNode();
                var portValues = new List<object> { 5, 3.5f };

                // Act
                var result = multiplyNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(17.5f, result);
            }

            [Test]
            public void MathBasicMultiplyNode_HandleOneNullValue()
            {
                // Arrange
                var multiplyNode = new RuntimeMathBasicMultiplyNode();
                var portValues = new List<object> { null, 3f };

                // Act
                var result = multiplyNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(0f, result);
            }

            [Test]
            public void MathBasicMultiplyNode_HandleAllNullValues()
            {
                // Arrange
                var multiplyNode = new RuntimeMathBasicMultiplyNode();
                var portValues = new List<object> { null, null };

                // Act
                var result = multiplyNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(0f, result);
            }
            #endregion

            #region Power
            [Test]
            public void MathBasicPowerNode_PowerTwoFloats()
            {
                // Arrange
                var powerNode = new RuntimeMathBasicPowerNode();
                var portValues = new List<object> { 2f, 3f };

                // Act
                var result = powerNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(8f, result);
            }

            [Test]
            public void MathBasicPowerNode_PowerIntAndFloat()
            {
                // Arrange
                var powerNode = new RuntimeMathBasicPowerNode();
                var portValues = new List<object> { 2, 3.5f };

                // Act
                var result = powerNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(11.313708f, (float)result, 0.000001f);
            }

            [Test]
            public void MathBasicPowerNode_HandleOneNullValue()
            {
                // Arrange
                var powerNode = new RuntimeMathBasicPowerNode();
                var portValues = new List<object> { null, 3f };

                // Act
                var result = powerNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(0f, result);
            }

            [Test]
            public void MathBasicPowerNode_HandleAllNullValues()
            {
                // Arrange
                var powerNode = new RuntimeMathBasicPowerNode();
                var portValues = new List<object> { null, null };

                // Act
                var result = powerNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(1f, result);
            }
            #endregion

            #region SquareRoot
            [Test]
            public void MathBasicSquareRootNode_SquareRootFloat()
            {
                // Arrange
                var squareRootNode = new RuntimeMathBasicSquareRootNode();
                var portValues = new List<object> { 16f };

                // Act
                var result = squareRootNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(4f, result);
            }

            [Test]
            public void MathBasicSquareRootNode_SquareRootInt()
            {
                // Arrange
                var squareRootNode = new RuntimeMathBasicSquareRootNode();
                var portValues = new List<object> { 16 };

                // Act
                var result = squareRootNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(4f, result);
            }

            [Test]
            public void MathBasicSquareRootNode_HandleAllNullValues()
            {
                // Arrange
                var squareRootNode = new RuntimeMathBasicSquareRootNode();
                var portValues = new List<object> { null };

                // Act
                var result = squareRootNode.Action(portValues, null);

                // Assert
                Assert.AreEqual(0f, result);
            }
            #endregion


    #endregion

    #endregion
}