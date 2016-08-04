class Message < ApplicationRecord
  belongs_to :machine, optional: true
  belongs_to :user, optional: true
end
