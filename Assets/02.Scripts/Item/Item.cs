public class Item
{
    public ItemData data;

    public Item(ItemData data)
    {
        this.data = data;
    }

    public void Use(Player player)
    {
        switch (data.type)
        {
            case ItemType.Potion:
                var potion = data as ItemData_Potion;
                player.condition.Heal(potion.healHpAmount);
                break;

            case ItemType.Weapon:
                var weapon = data as ItemData_Weapon;
                player.EquipWeapon(weapon);
                break;

            case ItemType.Armor:
                player.EquipArmor(data as ItemData_Armor);
                break;
        }
    }
}