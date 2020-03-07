using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Barcelleste
{
    public class Killable : MonoBehaviour
    {
        public void Die()
        {
            Destroy(gameObject);
        }
    }
}
