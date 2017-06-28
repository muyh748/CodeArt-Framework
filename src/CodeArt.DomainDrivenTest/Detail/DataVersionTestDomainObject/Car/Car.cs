﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Data;

using Dapper;

using CodeArt.TestTools;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using CodeArt.Runtime;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.DomainDrivenTest.Detail
{
    [ObjectRepository(typeof(ICarRepository), NoCache = true)]
    public class Car : AggregateRoot<Car, Guid>
    {
        #region 名称

        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, Car>("Name");

        [PropertyRepository()]
        [StringLength(1, 100)]
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            set
            {
                SetValue(NameProperty, value);
            }
        }

        #endregion

        #region 新旧标识

        private static readonly DomainProperty IsNewCarProperty = DomainProperty.Register<bool, Car>("IsNewCar");

        [PropertyRepository()]
        public bool IsNewCar
        {
            get
            {
                return GetValue<bool>(IsNewCarProperty);
            }
            set
            {
                SetValue(IsNewCarProperty, value);
            }
        }

        #endregion

        #region 基本值的集合1

        private static readonly DomainProperty LightCountsProperty = DomainProperty.RegisterCollection<int, Car>("LightCounts");

        [PropertyRepository()]
        public DomainCollection<int> LightCounts
        {
            get
            {
                return GetValue<DomainCollection<int>>(LightCountsProperty);
            }
            set
            {
                SetValue(LightCountsProperty, value);
            }
        }

        #endregion

        #region 基本值的集合2

        private static readonly DomainProperty ErrorMessagesProperty = DomainProperty.RegisterCollection<string, Car>("ErrorMessages");

        private DomainCollection<string> _ErrorMessages
        {
            get
            {
                return GetValue<DomainCollection<string>>(ErrorMessagesProperty);
            }
            set
            {
                SetValue(ErrorMessagesProperty, value);
            }
        }

        [PropertyRepository()]
        public IEnumerable<string> ErrorMessages
        {
            get
            {
                return _ErrorMessages;
            }
            set
            {
                _ErrorMessages =  new DomainCollection<string>(ErrorMessagesProperty, value);
            }
        }

        #endregion

        #region 基本值的集合3

        private static readonly DomainProperty DeliveryDatesProperty = DomainProperty.RegisterCollection<Emptyable<DateTime>, Car>("DeliveryDates");

        private DomainCollection<Emptyable<DateTime>> _DeliveryDates
        {
            get
            {
                return GetValue<DomainCollection<Emptyable<DateTime>>>(DeliveryDatesProperty);
            }
            set
            {
                SetValue(DeliveryDatesProperty, value);
            }
        }

        [PropertyRepository()]
        public IEnumerable<Emptyable<DateTime>> DeliveryDates
        {
            get
            {
                return _DeliveryDates;
            }
            set
            {
                _DeliveryDates = new DomainCollection<Emptyable<DateTime>>(DeliveryDatesProperty, value);
            }
        }

        public void AddDeliveryDate(Emptyable<DateTime> deliveryDate)
        {
            _DeliveryDates.Add(deliveryDate);
        }

        public void RemoveDeliveryDate(Emptyable<DateTime> deliveryDate)
        {
            _DeliveryDates.Remove(deliveryDate);
        }
        #endregion

        #region 简单值对象

        private static readonly DomainProperty AllColorProperty = DomainProperty.Register<WholeColor, Car>("AllColor", (owner) => WholeColor.Empty);

        [PropertyRepository()]
        public WholeColor AllColor
        {
            get
            {
                return GetValue<WholeColor>(AllColorProperty);
            }
            set
            {
                SetValue(AllColorProperty, value);
            }
        }


        #endregion

        #region 简单值对象的集合

        [PropertyRepository()]
        private static readonly DomainProperty CarAccessoriesProperty = DomainProperty.RegisterCollection<CarAccessory, Car>("CarAccessories");

        private DomainCollection<CarAccessory> _carAccessories
        {
            get
            {
                return GetValue<DomainCollection<CarAccessory>>(CarAccessoriesProperty);
            }
            set
            {
                SetValue(CarAccessoriesProperty, value);
            }
        }

        public IEnumerable<CarAccessory> CarAccessories
        {
            get
            {
                return _carAccessories;
            }
            set
            {
                _carAccessories = new DomainCollection<CarAccessory>(CarAccessoriesProperty, value);
            }
        }

        public void AddCarAccessory(CarAccessory carAccessory)
        {
            _carAccessories.Add(carAccessory);
        }

        public void RemoveCarAccessory(CarAccessory carAccessory)
        {
            _carAccessories.Remove(carAccessory);
        }
        #endregion

        #region 实体对象

        /// <summary>
        /// 引用对象
        /// </summary>

        private static readonly DomainProperty MainWheelProperty = DomainProperty.Register<CarWheel, Car>("MainWheel", CarWheel.Empty);

        [PropertyRepository()]
        public CarWheel MainWheel
        {
            get
            {
                return GetValue<CarWheel>(MainWheelProperty);
            }
            set
            {
                SetValue(MainWheelProperty, value);
            }
        }

        #endregion

        #region 实体对象的集合

        /// <summary>
        /// 引用对象的集合
        /// </summary>
        private static readonly DomainProperty WheelsProperty = DomainProperty.RegisterCollection<CarWheel, Car>("Wheels");

        private DomainCollection<CarWheel> _Wheels
        {
            get
            {
                return GetValue<DomainCollection<CarWheel>>(WheelsProperty);
            }
            set
            {
                SetValue(WheelsProperty, value);
            }
        }

        [PropertyRepository()]
        public IEnumerable<CarWheel> Wheels
        {
            get
            {
                return _Wheels;
            }
        }

        public void AddCarWheel(CarWheel wheel)
        {
            _Wheels.Add(wheel);
        }

        public void RemoveCarWheel(int wheelId)
        {
            var wheel = _Wheels.FirstOrDefault((t) => t.Id == wheelId);
            _Wheels.Remove(wheel);
        }

        #endregion

        #region 空对象

        private class CarEmpty : Car
        {
            public CarEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly Car Empty = new CarEmpty();

        #endregion


        public Car(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public Car(Guid id, DomainCollection<CarWheel> wheels)
            : base(id)
        {
            _Wheels = wheels;
            this.OnConstructed();
        }
    }

    /// <summary>
    /// 全车颜色
    /// </summary>
    [ObjectRepository(typeof(ICarRepository))]
    [ObjectValidator()]
    public class WholeColor : ValueObject
    {
        #region 名称

        public static readonly DomainProperty NameProperty = DomainProperty.Register<string, WholeColor>("Name");

        [PropertyRepository()]
        [StringLength(1, 150)]
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            private set
            {
                SetValue(NameProperty, value);
            }
        }

        #endregion

        #region 颜色数量

        private static readonly DomainProperty ColorNumProperty = DomainProperty.Register<int, WholeColor>("ColorNum");

        [PropertyRepository()]
        [IntRange(1, 100)]
        public int ColorNum
        {
            get
            {
                return GetValue<int>(ColorNumProperty);
            }
            private set
            {
                SetValue(ColorNumProperty, value);
            }
        }

        #endregion

        #region 已涂标识

        private static readonly DomainProperty IsPaintedProperty = DomainProperty.Register<bool, WholeColor>("IsPainted");

        [PropertyRepository()]
        public bool IsPainted
        {
            get
            {
                return GetValue<bool>(IsPaintedProperty);
            }
            private set
            {
                SetValue(IsPaintedProperty, value);
            }
        }

        #endregion

        #region 空对象

        private class WholeColorEmpty : WholeColor
        {
            public WholeColorEmpty()
                : base(string.Empty, 0, false)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly WholeColor Empty = new WholeColorEmpty();

        #endregion

        [ConstructorRepository]
        public WholeColor(string name, int colorNum, bool isPainted)
        {
            this.Name = name;
            this.ColorNum = colorNum;
            this.IsPainted = isPainted;
            this.OnConstructed();
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }

    /// <summary>
    /// 车辆配饰
    /// </summary>
    [ObjectRepository(typeof(ICarRepository))]
    [ObjectValidator()]
    public class CarAccessory : ValueObject
    {
        #region 名称

        public static readonly DomainProperty NameProperty = DomainProperty.Register<string, CarAccessory>("Name");

        [PropertyRepository()]
        [StringLength(1, 150)]
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            private set
            {
                SetValue(NameProperty, value);
            }
        }

        #endregion

        #region 配饰数量

        private static readonly DomainProperty AccessoryNumProperty = DomainProperty.Register<short, CarAccessory>("AccessoryNum");

        [PropertyRepository()]
        public short AccessoryNum
        {
            get
            {
                return GetValue<short>(AccessoryNumProperty);
            }
            private set
            {
                SetValue(AccessoryNumProperty, value);
            }
        }

        #endregion

        #region 装配日期

        private static readonly DomainProperty SetupDateProperty = DomainProperty.Register<Emptyable<DateTime>, CarAccessory>("SetupDate");

        [PropertyRepository()]
        public Emptyable<DateTime> SetupDate
        {
            get
            {
                return GetValue<Emptyable<DateTime>>(SetupDateProperty);
            }
            private set
            {
                SetValue(SetupDateProperty, value);
            }
        }

        #endregion

        #region 空对象

        private class CarAccessoryEmpty : CarAccessory
        {
            public CarAccessoryEmpty()
                : base(string.Empty, 0, null)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly CarAccessory Empty = new CarAccessoryEmpty();

        #endregion

        [ConstructorRepository]
        public CarAccessory(string name, short accessoryNum, Emptyable<DateTime> setupDate)
        {
            this.Name = name;
            this.AccessoryNum = accessoryNum;
            this.SetupDate = setupDate;
            this.OnConstructed();
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }

}