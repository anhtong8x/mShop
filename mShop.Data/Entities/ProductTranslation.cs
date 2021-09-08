using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.Data.Entities
{
    public class ProductTranslation
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        public string Details { set; get; }
        public string SeoDescription { set; get; }
        public string SeoTitle { set; get; }

        public string SeoAlias { get; set; }

        // khoa ngoai toi bang product
        public int ProductId { set; get; }

        // 1 product co nhieu producttranslation
        public Product Product { get; set; }

        // khoa ngoai toi bang
        public string LanguageId { set; get; }

        public Language Language { get; set; }
    }
}