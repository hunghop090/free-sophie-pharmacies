namespace Sophie.Resource.Model
{
    public enum TypeEnum
    {
        Actived,
        Draft,
        Trash
    }

    public enum TypePay
    {
        Other,
        Momo,
        Zalo,
        ZaloMomo
    }

    public enum TypePromotionsDiscount
    {
        //- giảm (zalo, momo): giảm 25% tối đa 25k, đơn hàng tối thiểu 90k
        TypePromotionsDiscount_1,

        //- giảm (zalo, momo): giảm 25k, đơn hàng tối thiểu 120k.
        TypePromotionsDiscount_2,

    }

    public enum TypeTransportPromotionsDiscount
    {
        //- miễn ship: đơn hàng tối thiểu 300k
        TypeTransportPromotionsDiscount_1,

        //- giảm ship: giảm 20k, đơn hàng tối thiểu 120k
        TypeTransportPromotionsDiscount_2,
    }

    public enum TypeStatusOrder
    {
        Pending,
        Successed,
        Failed,
    }
    
}
