using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
	ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

    public string SelectedGrupo { get; set; } = "Todos";
    public ListaProduto()
	{
		InitializeComponent();
        BindingContext = this;
        lst_produtos.ItemsSource = lista;
	}

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await LoadProdutos();
        ApplyGroupFilter(); // Aplica o filtro inicial
    }

    // Método para carregar todos os produtos do banco
    private async Task LoadProdutos()
    {
        try
        {
            lista.Clear();
            List<Produto> tmp = await App.Db.GetAll();
            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    
    private async void ApplyGroupFilter()
    {
        lista.Clear();

        List<Produto> produtosFiltrados;

        
        var produtos = await App.Db.GetAll();

        if (SelectedGrupo == "Todos")
        {
            
            produtosFiltrados = produtos;
        }
        else
        {
            
            produtosFiltrados = produtos.Where(p => p.Grupo == SelectedGrupo).ToList();
        }

        
        foreach (var produto in produtosFiltrados)
        {
            lista.Add(produto);
        }
    }


    private void pickerSearch_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectedGrupo = pickerSearch.SelectedItem.ToString();
        ApplyGroupFilter();
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
		try
		{
			Navigation.PushAsync(new Views.NovoProduto());
		}
		catch (Exception ex)
		{
			DisplayAlert("Ops", ex.Message, "OK");
		}

    }

	private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{

			string q = e.NewTextValue;

			lst_produtos.IsRefreshing = true;

			lista.Clear();

			List<Produto> tmp = await App.Db.Search(q);

			tmp.ForEach(i => lista.Add(i));
		}
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
		finally
		{
            lst_produtos.IsRefreshing = false;
        }
    }



    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
	{
		double soma = lista.Sum(i => i.Total);

		string msg = $"O total é {soma:C}";

		DisplayAlert("Total dos Produtos", msg, "OK");
	}

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
		try
		{
			MenuItem selecionado = sender as MenuItem;

			Produto p = selecionado.BindingContext as Produto;

			bool confirm = await DisplayAlert(
				"Tem certeza?", $"Remover {p.Descricao}?", "Sim", "Não");
			
			if (confirm)
			{
				await App.Db.Delete(p.Id);
				lista.Remove(p);
			}
		}
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
		try
		{
			Produto p = e.SelectedItem as Produto;

			Navigation.PushAsync(new Views.EditarProduto
			{
				BindingContext = p,
			});
		}

        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
		try
		{
			lista.Clear();
			List<Produto> tmp = await App.Db.GetAll();

			tmp.ForEach(i => lista.Add(i));
		}
		catch (Exception ex)
		{
			await DisplayAlert("Ops", ex.Message, "OK");
		}
		finally
		{
			lst_produtos.IsRefreshing = false;
		}
    }    
}