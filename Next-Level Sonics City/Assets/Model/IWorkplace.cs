using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public interface IWorkplace
    {
        public bool Employ(Person person);
        public void Fire(Person person);
    }
}