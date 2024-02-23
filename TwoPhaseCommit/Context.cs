using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunAlgorithm.Core;

namespace TwoPhaseCommit
{
    public class Context: IContext
    {
        private string? _tokenName;

        public IContext Clone()
        {
            return (IContext)MemberwiseClone();
        }

        public void GetToken(string name)
        {
            if (string.IsNullOrEmpty(_tokenName))
            {
                _tokenName = name;
                return;
            }

            throw new ApplicationException($"Token {name} rejected. Current: {_tokenName}");
        }

        public void FixToken(string name)
        {
            if (_tokenName == name )
            {
                return;
            }

            throw new ApplicationException($"Token {name} fix rejected. Current: {_tokenName}");
        }

        public override string ToString()
        {
            return _tokenName ?? "<empty>";
        }
    }
}
