class ChangeColumnType < ActiveRecord::Migration[5.0]
  def change
    change_column :messages, :user_id, :integer, required: false, null: true
  end
end
