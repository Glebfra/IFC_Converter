using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;

namespace IFC.Tools
{
    public static partial class IfcGeometry
    {
        public static IfcBooleanResult CreateBooleanResult(IModel model, IfcBooleanOperand firstOperand, IfcBooleanOperand secondOperand, IfcBooleanOperator @operator)
        {
            return model.Instances.New<IfcBooleanResult>(result =>
            {
                result.FirstOperand = firstOperand;
                result.SecondOperand = secondOperand;
                result.Operator = @operator;
            });
        }

        public static IfcBooleanResult CreateBooleanResult(IModel model, IEnumerable<IfcBooleanOperand> operands, IfcBooleanOperator @operator)
        {
            IfcBooleanOperand[] operandsArray = operands as IfcBooleanOperand[] ?? operands.ToArray();
            
            IfcBooleanResult booleanResult = CreateBooleanResult(model, operandsArray[0], operandsArray[1], @operator);
            for (int i = 2; i < operandsArray.Length; i++)
            {
                booleanResult = CreateBooleanResult(model, booleanResult, operandsArray[i], @operator);
            }

            return booleanResult;
        }
    }
}