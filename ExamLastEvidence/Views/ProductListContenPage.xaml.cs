using ExamLastEvidence.Services;
using ExamLastEvidence.ViewModels;
using System;
using System.Text.Json;

namespace ExamLastEvidence.Views;

public partial class ProductListContenPage : ContentPage
{
    private readonly ProductCollection productCollection;
    private readonly ProductService productService;
    private  List<CartItemViewModel> cart;
    public ProductListContenPage(ProductCollection productCollection, ProductService productService)
    {
        InitializeComponent();
        this.productCollection = productCollection;
        this.productService = productService;
        CallGetProducts();
        this.BindingContext = this.productCollection;
        cart = new List<CartItemViewModel>();
        ReadCart();
    }
    private async void CallGetProducts()
    {
        await GetProducts();


    }
    private async Task GetProducts()
    {

        var data = await this.productService.Get();
        if (data == null) { return; }
        foreach (var item in data)
        {

            productCollection.Add(item);
        }
    }

    private void ProductTapItem_Tapped(object sender, TappedEventArgs e)
    {
        var tappedItem = sender as Frame;
        var data = tappedItem?.BindingContext as ProductViewModel;
        
    }

    private void AddToCart_Clicked(object sender, EventArgs e)
    {
        var btn = sender as Button;
        var id = int.Parse(btn?.CommandParameter.ToString() ?? "0");
        var item = cart.Find(x => x.ProductId == id);
        if(item == null) {
            cart.Add(new CartItemViewModel {  ProductId=id, Quantity = 1 });
        }
        else
        {
            item.Quantity += 1;
        }
        WriteCart();
    }
    private void ReadCart()
    {
        if (File.Exists(Path.Combine(FileSystem.AppDataDirectory, "cart.json")))
        {
            var cartString = File.ReadAllText(Path.Combine(FileSystem.AppDataDirectory, "cart.json"));
            var data = JsonSerializer.Deserialize<List<CartItemViewModel>>(cartString);
            if (data == null)
            {
               
            }
            else
            {
                cart = data;
            }
        }
    }
    public void WriteCart()
    {
        string cartString = JsonSerializer.Serialize(cart);
        File.WriteAllText(Path.Combine(FileSystem.AppDataDirectory, "cart.json"), cartString);
    }
}