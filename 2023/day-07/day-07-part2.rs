use std::cmp::Ordering;
use std::env;
use std::fs;

const CARD_VALUES: [char; 13] = [
    'J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A',
];

struct CardsGroup {
    symbol: char,
    ammount: i32,
}

trait CompareCardsGroup {
    fn compare(&self, other: &CardsGroup) -> Ordering;
}

impl CompareCardsGroup for CardsGroup {
    fn compare(&self, other: &CardsGroup) -> Ordering {
        if self.ammount > other.ammount {
            return Ordering::Greater;
        } else if self.ammount < other.ammount {
            return Ordering::Less;
        }
        return Ordering::Equal;
    }
}

struct Hand {
    cards: String,
    groups: Vec<CardsGroup>,
    bid: i64,
}

trait CompareHand {
    fn compare(&self, other: &Hand) -> Ordering;
}

impl CompareHand for Hand {
    fn compare(&self, other: &Hand) -> Ordering {
        let mut self_index = 0;
        let mut other_index = 0;

        while self_index < self.groups.len() && other_index < other.groups.len() {
            let self_card = &self.groups[self_index];
            let other_card = &other.groups[other_index];

            let compare = self_card.compare(other_card);

            if compare == Ordering::Greater {
                return Ordering::Greater;
            } else if compare == Ordering::Less {
                return Ordering::Less;
            }

            self_index += 1;
            other_index += 1;
        }

        self_index = 0;
        other_index = 0;
        while self_index < self.cards.len() && other_index < other.cards.len() {
            let self_card = self.cards.chars().nth(self_index).unwrap();
            let other_card = other.cards.chars().nth(other_index).unwrap();

            let self_card_index = CARD_VALUES.iter().position(|&r| r == self_card).unwrap();
            let other_card_index = CARD_VALUES.iter().position(|&r| r == other_card).unwrap();

            if self_card_index > other_card_index {
                return Ordering::Greater;
            } else if self_card_index < other_card_index {
                return Ordering::Less;
            }

            self_index += 1;
            other_index += 1;
        }
        return Ordering::Equal;
    }
}

fn main() {
    let args: Vec<String> = env::args().collect();
    let file_path: &str;

    if args.len() > 1 {
        file_path = &args[1];
    } else {
        file_path = "input.txt";
    }

    let file = fs::read_to_string(file_path);

    if let Ok(contents) = file {
        let mut hands: Vec<Hand> = Vec::new();
        for line in contents.lines() {
            if line.len() == 0 {
                continue;
            }

            let line_split: Vec<&str> = line.split(" ").collect();

            let mut vec: Vec<CardsGroup> = Vec::new();
            let mut count_jokers = 0;
            for c in line_split[0].chars() {
                if c == 'J' {
                    count_jokers += 1;
                    continue;
                }
                let aux = vec.iter().position(|t| t.symbol == c);
                if aux.is_none() {
                    vec.push(CardsGroup {
                        symbol: c,
                        ammount: 1,
                    });
                } else {
                    let index = aux.unwrap();
                    vec[index] = CardsGroup {
                        symbol: c,
                        ammount: vec[index].ammount + 1,
                    }
                }
            }

            vec.sort_by(|a, b| b.compare(&a));
            if vec.len() != 0 {
                vec[0] = CardsGroup {
                    symbol: vec[0].symbol,
                    ammount: vec[0].ammount + count_jokers,
                };
            } else {
                vec.push(CardsGroup {
                    symbol: 'J',
                    ammount: count_jokers,
                });
            }

            hands.push(Hand {
                cards: String::from(line_split[0]),
                groups: vec,
                bid: line_split[1].parse().unwrap(),
            });
        }

        hands.sort_by(|a, b| a.compare(b));

        let mut sum = 0;
        let mut i = 0;

        while i < hands.len() {
            let rank = i as i64 + 1;
            sum = sum + hands[i].bid * rank;
            i += 1;
        }

        println!("Total winnings: {}", sum);
    }
}
