namespace eCommerceWebAPI.Application.Extensions
{
    public enum StringCaseEnum
    {
        ShuffleCaseWithoutSpace = 110,
        ShuffleCaseSpaceBetween = 120,
        ShuffleSnakeCase = 130,
        ShuffleKebabCase = 140,

        LowerCaseWithoutSpace = 210,
        LowerCaseSpaceBetween = 220,
        LowerCaseSpaceBetweenFirstCharUpperCase = 230,

        UpperCaseWithoutSpace = 310,
        UpperCaseSpaceBetween = 320,

        PascalCaseWithoutSpace = 410,
        PascalCaseSpaceBetween = 420,

        CamelCaseWithoutSpace = 510,
        CamelCaseSpaceBetween = 520,

        SnakeLowerCase = 610,
        SnakeUpperCase = 620,
        SnakePascalCase = 630,
        SnakeCamelCase = 640,

        KebabLowerCase = 750,
        KebabUpperCase = 760,
        KebabPascalCase = 770,
        KebabCamelCase = 780
    }
}
