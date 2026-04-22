using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using AutoMapper;

using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;

namespace DRLMobile.Core.AutoMapperProfiler
{

    public class ProfileMatcher
    {
        private static ProfileMatcher _instance = null;
        private ProfileMatcher() { }

        public static ProfileMatcher GetInstance
        {
            get
            {
                if (_instance == null)
                    _instance = new ProfileMatcher();
                return _instance;
            }
        }

        public IMapper GetMapper(Func<IMapperConfigurationExpression, IMapperConfigurationExpression> fn)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg = fn(cfg);
            });
            return new Mapper(config);
        }

        public IMapperConfigurationExpression CreateOrderDetailMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<OrderDetailUIModel, OrderDetail>().ReverseMap()
             .ForMember(dest => dest.QuantityDisplay, act => act.MapFrom(src => src.Quantity))
             .ForMember(dest => dest.ProductID, act => act.MapFrom(src => src.ProductId))
             .ForMember(dest => dest.ItemDescription, act => act.MapFrom(src => src.ProductDescription))
             .ForMember(dest => dest.ItemNumber, act => act.MapFrom(src => src.ProductName));
            return cfg;
        }
        public IMapperConfigurationExpression CreateCategoryDataMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<CategoryUIModel, CategoryMaster>().ReverseMap()
            .ForMember(dest => dest.CategoryId, act => act.MapFrom(src => src.CategoryID))
            .ForMember(dest => dest.CategoryImage, act =>
                 act.MapFrom(src => (src != null) ? "ms-appx:///Assets/SRCProduct/category_unselected.png" : ""));

            return cfg;
        }

        public IMapperConfigurationExpression CreateBrandDataMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<BrandUIModel, BrandData>().ReverseMap()
                .ForMember(dest => dest.BrandImageFromLocal,
                act => act.MapFrom(src =>
                        Path.Combine(ApplicationConstants.APP_PATH,
                        ApplicationConstants.BrandImageBaseFolder,
                   HelperMethods.GetNameFromURL(src.ImageFileName)))
            );
            return cfg;
        }

        public IMapperConfigurationExpression CreateStyleDataMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<StyleUIModel, StyleData>().ReverseMap()
                 .ForMember(dest => dest.StyleImage, act =>
                 act.MapFrom(src => (src != null) ? "ms-appx:///Assets/SRCProduct/style_unselected.png" : ""));
            return cfg;
        }

        public IMapperConfigurationExpression CreateSRCProductsPageDataMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<SRCProductUIModel, ProductMaster>().ReverseMap()
            .ForMember(dest => dest.ItemNumber, act => act.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.ItemDescription, act => act.MapFrom(src => src.Description))
            .ForMember(dest => dest.IsDistributed, act => act.MapFrom(src => src.isDistributed))
            .ForMember(dest => dest.DistributionRecordedDate, act => act.MapFrom(src =>
            Helpers.DateTimeHelper.ConvertEmptyStringDateToMM_DD_YYYY(src.DistributionRecordedDate)))
                .ForMember(dest => dest.Link, act => act.MapFrom(src => src.Description))
                .ForMember(dest => dest.IsFavorite, act => act.MapFrom(src => src.FavoriteData))
                .ForMember(dest => dest.IsAddedToCart, act => act.MapFrom(src => src.CartData))
                .ForMember(dest => dest.Quantity, act => act.MapFrom(src => src.CartQuantity));
            return cfg;
        }

        public IMapperConfigurationExpression CreateCustomerMasterMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<SavedCustomerInfoUIModel, CustomerMaster>().ReverseMap();
            return cfg;
        }

        public IMapperConfigurationExpression CreateOrderHistoryDetailsGridMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<OrderHistoryDetailsGridUIModel, OrderDetail>().ReverseMap()
            .ForMember(dest => dest.IsTobbaco, act => act.MapFrom(src => src.isTobbaco))
            .ForMember(dest => dest.ProductQty, act => act.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.DisplayProductQty, act => act.MapFrom(src => Convert.ToString(src.Quantity)))
            .ForMember(dest => dest.DisplayProductUnitPrice, act => act.MapFrom(src =>
            string.IsNullOrEmpty(src.Price) ? "$0.00" : string.Format("${0}", Convert.ToDecimal(src.Price).ToString("0.00"))))
            .ForMember(dest => dest.ProductUnitPrice, act => act.MapFrom(src =>
            (string.IsNullOrEmpty(src.Price) ? 0 : Convert.ToDecimal(src.Price))))
            //.ForMember(dest => dest.OrderDetailObject, act => act.MapFrom(src => src))
            .ForMember(dest => dest.DisplayProductSubtotal, act => act.MapFrom((src) => CalculateSubTotal(src)))
            .ForMember(dest => dest.CreditRequestType, act => act.MapFrom(src => src.CreditRequest))
            .ForMember(dest => dest.DisplayProductUnit, act => act.MapFrom(src => src.Unit))
            .ForMember(dest => dest.DisplayBrandName, act => act.MapFrom(src => src.BrandName))
            .ForMember(dest => dest.DisplayProductDesc, act => act.MapFrom(src => src.ProductDescription))
            .ForMember(dest => dest.DisplayProductName, act => act.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.DisplayStyleName, act => act.MapFrom(src => src.StyleName));
            return cfg;
        }

        public IMapperConfigurationExpression CreateProductDistributionMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<SRCProductUIModel, ProductDistribution>().ReverseMap()
                 .ForMember(dest => dest.IsDistributed, act => act.MapFrom(src => 1))
                  .ForMember(dest => dest.DistributionRecordedDate, act => act.MapFrom(src => DateTimeHelper.ConvertStringDateToMM_DD_YYYY(src.DistributionDate)));
            return cfg;
        }

        public IMapperConfigurationExpression CreateSRCProductMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<SRCProductUIModel, ProductMaster>().ReverseMap()
            .ForMember(dest => dest.ItemNumber, act => act.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.ItemDescription, act => act.MapFrom(src => src.Description))
            .ForMember(dest => dest.IsDistributed, act => act.MapFrom(src =>
            src.isDistributed))
            .ForMember(dest => dest.Link, act => act.MapFrom(src => src.Description))
            .ForMember(dest => dest.IsFavorite, act => act.MapFrom(src => src.FavoriteData))
            .ForMember(dest => dest.IsAddedToCart, act => act.MapFrom(src => src.CartData))
            .ForMember(dest => dest.Quantity, act => act.MapFrom(src => src.CartQuantity));
            return cfg;
        }

        private string CalculateSubTotal(OrderDetail src)
        {
            decimal deciamlPrice = 0;
            if (!string.IsNullOrEmpty(src.Price))
            {
                deciamlPrice = Convert.ToDecimal(src.Price);
            }
            return string.Format("${0}", (src.Quantity * deciamlPrice).ToString("0.00"));
        }
    }

}
